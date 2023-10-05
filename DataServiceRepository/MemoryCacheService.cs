using DataModels;
using DataServiceRepository.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceRepository
{
    public class MemoryCacheService<T> : IMemoryCacheService<T> where T : class
    {
        private IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T GetValueFromCache(string key)
        {
            T value;
            _cache.TryGetValue(key, out value);
            return value;
        }

        public void SetValueInCache(string key, T value)
        {
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                Priority = CacheItemPriority.High,
                //SlidingExpiration = TimeSpan.FromSeconds(20)
            };
            //setting cache entries
            _cache.Set(key, value, cacheExpiryOptions);
        }
    }
}
