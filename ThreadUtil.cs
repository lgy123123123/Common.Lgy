using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.CustomException;
namespace Common
{
    /// <summary>
    /// Thread类辅助使用，可立即停止其他无用线程
    /// </summary>
    public class ThreadUtil
    {
        #region 属性
        /// <summary>
        /// 线程集合
        /// </summary>
        public List<Thread> listTh { get; set; } = new List<Thread>();
        /// <summary>
        /// 等待线程状态集合
        /// </summary>
        public List<AutoResetEvent> listAutoEvent { get; set; } = new List<AutoResetEvent>();
        /// <summary>
        /// 某个线程出现异常，立即停止其他线程
        /// </summary>
        public bool IsExpStop { get; set; } = true;
        /// <summary>
        /// 第一个异常
        /// </summary>
        public Exception ExceptionFirst { get { return listException.Count > 0 ? listException.First() : null; } }
        /// <summary>
        /// 所有异常的集合
        /// </summary>
        private List<Exception> listException { get; set; } = new List<Exception>();
        /// <summary>
        /// 是否已停止所有进程
        /// </summary>
        private bool HaveStop { get; set; } = false;
        /// <summary>
        /// 所有线程是否执行完成
        /// </summary>
        public bool IsFinish
        {
            get
            {
                bool ret = true;
                foreach (Thread th in listTh)
                {
                    if (th.IsAlive)
                    {
                        ret = false;
                        break;
                    }
                }
                return ret;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        ///
        /// </summary>
        /// <param name="isHasExceptionStop">遇到异常是否立即停止其他线程，默认停止(true)</param>
        public ThreadUtil(bool isHasExceptionStop = true)
        {
            IsExpStop = isHasExceptionStop;
        }
        #endregion
        /// <summary>
        /// 添加线程
        /// </summary>
        /// <param name="a">线程执行代码</param>
        /// <param name="isBackground">是否是后台线程</param>
        /// <param name="isImmidiateStart">是否立即启动，默认立即启动</param>
        public void AddThread(Action a,bool isBackground=true, bool isImmidiateStart = true)
        {
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            listAutoEvent.Add(autoEvent);
            Thread t = new Thread(delegate ()
            {
                try
                {
                    a();
                    autoEvent.Set();
                }
                catch (Exception e)
                {
                    lock (listException)
                    {
                        listException.Add(e);
                            //停止所有线程
                            if (IsExpStop && !HaveStop)
                            StopAll(Thread.CurrentThread);
                    }
                }
            });
            t.IsBackground = isBackground;
            listTh.Add(t);
            if (isImmidiateStart)
                t.Start();
        }
        /// <summary>
        /// 开始执行所有线程
        /// </summary>
        public void StartAll()
        {
            listTh.ForEach(t =>
            {
                if (!t.IsAlive)
                    t.Start();
            });
        }
        /// <summary>
        /// 等待所有线程结束，结束后可自动抛出异常
        /// </summary>
        /// <param name="autoThrowException">线程结束后自动抛出异常，默认true</param>
        /// <param name="millsecondtimeout">超时时间，默认是-1永不超时</param>
        /// <returns>返回是否正常结束  true正常结束  false，超时停止</returns>
        public bool WaitAll(bool autoThrowException = true, int millsecondtimeout = -1)
        {
            if (listAutoEvent.Count == 0)
                return true;
            bool isNotTimeOut = WaitHandle.WaitAll(listAutoEvent.ToArray(), millsecondtimeout);
            if (!isNotTimeOut)
            {
                StopAll();
                if (autoThrowException)
                    throw new BaseException("执行超时");
            }
            if (autoThrowException && ExceptionFirst != null)
                throw ExceptionFirst;
            return isNotTimeOut;
        }
        /// <summary>
        /// 停止所有线程
        /// </summary>
        /// <param name="currentThread">当前线程，不会停止当前线程</param>
        public void StopAll(Thread currentThread = null)
        {
            if (HaveStop)
                return;
            HaveStop = true;
            for (int i = 0; i < listTh.Count; i++)
            {
                listAutoEvent[i].Set();
                if (listTh[i].IsAlive && currentThread != null && listTh[i].ManagedThreadId == currentThread.ManagedThreadId)
                    continue;
                listTh[i].Abort();
            }
        }
    }
}
