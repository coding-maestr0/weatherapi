using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceRepository.Interfaces
{
    public interface IMemoryCacheService<T> where T : class
    {
        T GetValueFromCache(string key);
        void SetValueInCache(string key, T value);
    }
}
