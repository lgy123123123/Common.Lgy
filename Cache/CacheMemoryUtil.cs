using System;
using Common.CustomException;
using Microsoft.Extensions.Caching.Memory;

namespace Common
{
    /// <summary>
    /// 内存缓存帮助类
    /// </summary>
    public class CacheMemoryUtil:ICacheUtil
    {
        /// <summary>
        /// 缓存对象
        /// </summary>
        private MemoryCache Cache { get; set; }
        /// <summary>
        /// 缓存参数
        /// </summary>
        private MemoryCacheOptions option { get; set; }
        /// <summary>
        /// 构造内存缓存对象
        /// </summary>
        /// <param name="_option">参数</param>
        public CacheMemoryUtil(MemoryCacheOptions _option=null)
        {
            if(_option is null)
                _option=new MemoryCacheOptions();
            option = _option;
            Cache = new MemoryCache(option);
        }
        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (key.IsNotNullOrEmpty())
            {
                return Cache.TryGetValue(key, out _);
            }
            return false;
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSliding">滑动过期时间(一段内不访问，则清空缓存，访问后，按滑动时间重新计算)，null则不设置</param>
        /// <param name="expiressAbsoulte">绝对过期时间，null则不设置</param>
        /// <returns></returns>
        public void Set(string key, object value, TimeSpan? expiresSliding=null, DateTime? expiressAbsoulte=null)
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = expiresSliding,
                AbsoluteExpiration = expiressAbsoulte
            };
            Cache.Set(key, value, options);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="BaseException">缓存不存在</exception>
        public T Get<T>(string key)
        {
            if (!Exists(key))
                //throw new BaseException("查找的缓存不存在");
                return default(T);
            return Cache.Get<T>(key);
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="BaseException">缓存不存在</exception>
        public object Get(string key)
        {
            if (!Exists(key))
                return null;
            return Cache.Get(key);
        }
        /// <summary>
        /// 获取字符串缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            return Cache.Get(key).ToString();
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (Exists(key))
                Cache.Remove(key);
        }
        /// <summary>
        /// 释放缓存
        /// </summary>
        public void Dispose()
        {
            Cache.Dispose();
        }

        /// <summary>
        /// 重新创建缓存
        /// </summary>
        public void Rebuild()
        {
            Dispose();
            Cache = new MemoryCache(option);
        }
    }
}
