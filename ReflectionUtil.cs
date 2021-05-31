using System;
using System.Reflection;
using Common.CustomException;

namespace Common
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public static class ReflectionUtil
    {
        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="dll">程序集名称/dll名称/dll全路径</param>
        /// <returns></returns>
        public static Assembly GetAssembly(string dll)
        {
            Assembly ass;
            if (dll.IndexOf(".dll", StringComparison.OrdinalIgnoreCase) >= 0)
                ass = Assembly.LoadFrom(dll);
            else
                ass = Assembly.Load(dll);
            return ass;
        }
        /// <summary>
        /// 执行dll
        /// </summary>
        /// <param name="dllPath">dll名称或路径</param>
        /// <param name="className">全路径类名</param>
        /// <param name="funcName">方法名</param>
        /// <param name="param">执行参数</param>
        /// <returns></returns>
        public static object ExecDll(string dllPath, string className, string funcName, params object[] param)
        {
            Assembly assembly = GetAssembly(dllPath);
            return ExecAssembly(assembly, className, funcName, param);
        }
        /// <summary>
        /// 执行程序集
        /// </summary>
        /// <param name="assembly">程序集对象</param>
        /// <param name="className">全路径类名</param>
        /// <param name="funcName">方法名</param>
        /// <param name="param">执行参数</param>
        /// <returns></returns>
        public static object ExecAssembly(Assembly assembly, string className, string funcName, params object[] param)
        {
            object ret = null;
            Type t = assembly.GetType(className);
            if (t is null)
                throw new BaseException($"反射错误：未找到名为{className}的类型");
            MethodInfo method = t.GetMethod(funcName);
            if (method is null)
                throw new BaseException($"反射错误：未找到名为{funcName}的方法名");
            if (method.IsStatic)
            {
                ret = method.Invoke(null, param);
            }
            else
            {
                var instance = Activator.CreateInstance(t);
                ret = method.Invoke(instance, param);
            }
            return ret;
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ass">程序集</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="param">实例化参数</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static T GetInstance<T>(Assembly ass, string className, object[] param, bool ignoreCase = true) where T : class
        {
            Type t = ass.GetType(className, false, ignoreCase);
            if (t == null)
                throw new BaseException($"反射错误：未找到名为{className}的类型");
            return (T)Activator.CreateInstance(t, param);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dllName">dll的名称，或者路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="param">实例化参数</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static T GetInstance<T>(string dllName, string className, object[] param, bool ignoreCase = true) where T : class
        {
            Assembly ass = GetAssembly(dllName);
            if (ass is null)
                throw new BaseException($"反射错误：未找到名为{dllName}的程序集");
            return GetInstance<T>(ass, className, param, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dllName">dll的名称，或者路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static T GetInstance<T>(string dllName, string className, bool ignoreCase = true) where T : class
        {
            return GetInstance<T>(dllName, className, null, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ass">程序集</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static T GetInstance<T>(Assembly ass, string className, bool ignoreCase = true) where T : class
        {
            return GetInstance<T>(ass, className, null, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <param name="dllName">dll的名称，或者路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="param">实例化参数</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static object GetInstance(string dllName, string className, object[] param, bool ignoreCase = true)
        {
            return GetInstance<object>(dllName, className, param, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <param name="ass">程序集</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="param">实例化参数</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static object GetInstance(Assembly ass, string className, object[] param, bool ignoreCase = true)
        {
            return GetInstance<object>(ass, className, param, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <param name="dllName">dll的名称，或者路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static object GetInstance(string dllName, string className, bool ignoreCase = true)
        {
            return GetInstance<object>(dllName, className, null, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <param name="ass">程序集</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static object GetInstance(Assembly ass, string className, bool ignoreCase = true)
        {
            return GetInstance<object>(ass, className, null, ignoreCase);
        }
    }
}
