using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.CustomException;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Common
{
    /// <summary>
    /// 定时帮助类，基于Quartz.net
    /// </summary>
    public static class CronUtil
    {
        /// <summary>
        /// 调度
        /// </summary>
        public static IScheduler sche { get; set; }
        /// <summary>
        /// 日志文件名
        /// </summary>
        public static string LogFileName = "Log";
        /// <summary>
        /// job运行记录日志的文件夹名
        /// </summary>
        public static string LogDirName = "CronUtil";

        /// <summary>
        /// 启动调度的时候，锁定用
        /// </summary>
        private static Object lockObj = new Object();
        /// <summary>
        /// 初始化调度
        /// </summary>
        public static async void InitScheduler()
        {
            if (sche == null)
            {
                ISchedulerFactory factory = new StdSchedulerFactory();
                sche = await factory.GetScheduler();
                await sche.Start();
                LogUtil.AddLog("job启动", LogFileName, LogDirName);
            }
        }
        /// <summary>
        /// 所有正在运行的Job列表
        /// </summary>
        public static ConcurrentDictionary<string,JobKey> dicAllJob { get; set; } = new ConcurrentDictionary<string, JobKey>();

        /// <summary>
        /// 添加cron定时job
        /// </summary>
        /// <param name="name">job名称，不能重复</param>
        /// <param name="cron">cron表达式</param>
        /// <param name="doing">需要执行的方法</param>
        /// <param name="runNow">即使未到cron时间，也立即执行一次，默认false，不立即执行</param>
        /// <param name="isContinue">上次未执行完，本次是否触发执行，默认false等待上次执行结束</param>
        public static void AddCronJob(string name, string cron, Action doing, bool runNow = false, bool isContinue = false)
        {
            //创建cron触发器
            var tri = TriggerBuilder.Create();
            tri.WithCronSchedule(cron);
            //验证cron表达式是否正确
            var cronList = TriggerUtils.ComputeFireTimes(tri.WithCronSchedule(cron).Build() as IOperableTrigger, null, 1);
            if (cronList.Count == 0)
                throw new BaseException($"添加job出现错误：{name}的cron表达式不正确");
            //立即运行，先运行一次，然后再添加到cron定时
            if (runNow)
            {
                Thread th = new Thread(() =>
                {
                    try
                    {
                        Thread.CurrentThread.IsBackground = false;
                        doing();
                    }
                    catch (Exception e)
                    {
                        LogUtil.AddErrorLog($"job运行出错，job名称：{name}", CronUtil.LogFileName, CronUtil.LogDirName, e);
                    }
                    finally
                    {
                        AddJob(name, tri, doing, isContinue);
                    }
                });
                th.IsBackground = false;
                th.Start();
            }
            else
                AddJob(name, tri, doing, isContinue);
        }
        /// <summary>
        /// 添加间隔小时定时,添加后立即启动
        /// </summary>
        /// <param name="name">job名称，不能重复</param>
        /// <param name="hour">小时间隔</param>
        /// <param name="doing">需要执行的方法</param>
        /// <param name="repeatCount">重复次数，-1为永远重复,0只执行一次</param>
        /// <param name="isContinue">上次未执行完，本次是否触发执行，默认false等待上次执行结束</param>
        public static void AddHourJob(string name, int hour, Action doing, int repeatCount = -1, bool isContinue = false)
        {
            TriggerBuilder tri = TriggerBuilder.Create();
            if (repeatCount == -1)
                tri.WithSimpleSchedule((b) => b.WithIntervalInHours(hour).RepeatForever());
            else
                tri.WithSimpleSchedule((b) => b.WithIntervalInHours(hour).WithRepeatCount(repeatCount));
            AddJob(name, new List<TriggerBuilder> { tri }, doing, isContinue); ;
        }
        /// <summary>
        /// 添加间隔分钟定时,添加后立即启动
        /// </summary>
        /// <param name="name">job名称，不能重复</param>
        /// <param name="minute">分钟间隔</param>
        /// <param name="doing">需要执行的方法</param>
        /// <param name="repeatCount">重复次数，-1为永远重复,0只执行一次</param>
        /// <param name="isContinue">上次未执行完，本次是否触发执行，默认false等待上次执行结束</param>
        public static void AddMinuteJob(string name, int minute, Action doing, int repeatCount = -1, bool isContinue = false)
        {
            TriggerBuilder tri = TriggerBuilder.Create();
            if (repeatCount == -1)
                tri.WithSimpleSchedule((b) => b.WithIntervalInMinutes(minute).RepeatForever());
            else
                tri.WithSimpleSchedule((b) => b.WithIntervalInMinutes(minute).WithRepeatCount(repeatCount));
            AddJob(name, new List<TriggerBuilder> { tri }, doing, isContinue); ;
        }
        /// <summary>
        /// 添加间隔秒定时,添加后立即启动
        /// </summary>
        /// <param name="name">job名称，不能重复</param>
        /// <param name="second">秒间隔</param>
        /// <param name="doing">需要执行的方法</param>
        /// <param name="repeatCount">重复次数，-1为永远重复,0只执行一次</param>
        /// <param name="isContinue">上次未执行完，本次是否触发执行，默认false等待上次执行结束</param>
        public static void AddSecondJob(string name, int second, Action doing, int repeatCount = -1, bool isContinue = false)
        {
            TriggerBuilder tri = TriggerBuilder.Create();
            if (repeatCount == -1)
                tri.WithSimpleSchedule((b) => b.WithIntervalInSeconds(second).RepeatForever());
            else
                tri.WithSimpleSchedule((b) => b.WithIntervalInSeconds(second).WithRepeatCount(repeatCount));
            AddJob(name, new List<TriggerBuilder> { tri }, doing, isContinue); ;
        }
        /// <summary>
        /// 添加定时job
        /// </summary>
        /// <param name="name">job名称，不能重复</param>
        /// <param name="tri">触发器</param>
        /// <param name="doing">需要执行的方法</param>
        /// <param name="isContinue">上次未执行完，本次是否触发执行，默认false等待上次执行结束</param>
        public static void AddJob(string name, TriggerBuilder tri, Action doing, bool isContinue = false)
        {
            AddJob(name, new List<TriggerBuilder> { tri }, doing, isContinue);
        }
        /// <summary>
        /// 添加定时job
        /// </summary>
        /// <param name="name">job名称，不能重复</param>
        /// <param name="listTri">触发器们</param>
        /// <param name="doing">需要执行的方法</param>
        /// <param name="isContinue">上次未执行完，本次是否触发执行，默认false等待上次执行结束</param>
        public static async void AddJob(string name, List<TriggerBuilder> listTri, Action doing, bool isContinue = false)
        {
            if (sche is null)
            {
                lock (lockObj)
                {
                    InitScheduler();
                }
            }
            if (listTri.Count == 0)
                throw new BaseException($"添加job出现错误：{name}，未填写触发器");
            //验证重复job
            if(dicAllJob.ContainsKey(name))
                throw new BaseException($"添加job出现错误：{name}重复");
            //创建job
            IJobDetail job = new JobDetailImpl(name, isContinue ? typeof(ExecuteJobContinue) : typeof(ExecuteJob));
            job.JobDataMap.Add("doing", doing);
            //添加第一个job
            await sche.ScheduleJob(job, listTri[0].Build());
            //添加其他job  第二个触发器的创建方式不同
            for (int i = 1; i < listTri.Count; i++)
            {
                await sche.ScheduleJob(listTri[i].ForJob(job).Build());
            }
            if(!dicAllJob.TryAdd(name,job.Key))
                throw new BaseException($"job加入集合失败：{name}");
        }
        /// <summary>
        /// 停止job
        /// </summary>
        /// <param name="name"></param>
        public static async void StopJob(string name)
        {
            try
            {
                if (dicAllJob.ContainsKey(name))
                {
                    JobKey key = dicAllJob[name];
                    await sche.Interrupt(key);
                    await sche.DeleteJob(key);
                    if(!dicAllJob.TryRemove(name,out _))
                        throw new Exception($"停止job失败：{name}");
                    else
                        LogUtil.AddLog($"停止了job：{name}", LogFileName, LogDirName);
                }
            }
            catch (Exception e)
            {
                LogUtil.AddErrorLog(e.Message, LogFileName, LogDirName, e);
                throw;
            }
        }
    }
    /// <summary>
    /// 正常执行的类
    /// </summary>
    class ExecuteJobContinue : IJob
    {
        private Thread _currentThread;
        private string name;
        /// <summary>
        /// 执行入口 
        /// </summary>
        /// <param name="context"></param>
        public Task Execute(IJobExecutionContext context)
        {
            _currentThread = Thread.CurrentThread;
            try
            {
                var doing = context.JobDetail.JobDataMap.Get("doing") as Action;
                name = context.JobDetail.Key.Name;
                doing?.Invoke();
            }
            catch (Exception e)
            {
                LogUtil.AddErrorLog($"job运行出错，job名称：{name}，" + e.Message, CronUtil.LogFileName, CronUtil.LogDirName, e);
            }
            finally
            {
                _currentThread = null;
            }
            return null;
        }
    }
    /// <summary>
    /// 上次未执行完，本次不执行
    /// </summary>
    [DisallowConcurrentExecution]
    class ExecuteJob : ExecuteJobContinue
    { }
}
