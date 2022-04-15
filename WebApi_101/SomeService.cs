using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_101
{
    public class SomeService : ISomeService
    {
        private readonly static MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        private static DateTime DateTime = DateTime.Now;

        public SomeService()
        {
            //memoryCache.Set("key1", "this is default");
        }

        public string GetStaticVariable()
        {
            return DateTime.ToString();
        }

        public string GetPersisted()
        {
            return Convert.ToString(memoryCache.Get("key1"));
        }

        public void SetPersisted(string data)
        {
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(3),
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(3),
            };

            Task.Delay(5000).Wait();


            memoryCache.Set("key1", data, options);
        }

        public void SetNow(string data)
        {
            memoryCache.Set("key1", data);
        }
    }

    public interface ISomeService
    {
        void SetPersisted(string data);
        void SetNow(string data);
        string GetPersisted();
        string GetStaticVariable();
    }
}
