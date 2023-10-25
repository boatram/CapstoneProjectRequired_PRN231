using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace Repository.Extensions
{
   
    public class CacheService : ICacheService
    {
        private ObjectCache _cache=MemoryCache.Default;
      
        public T GetData<T>(string key)
        {
            try
            {
                T item = (T)_cache.Get(key);
                return item;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object RemoveData(string key)
        {
            try
            {
                var _exist = _cache.Get(key);
                if (_exist != null)
                    return _cache.Remove(key);
                return false;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            try
            {
                var expirty = expirationTime.DateTime.Subtract(DateTime.Now);
                _cache.Set(key, value, expirationTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
