using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace TheAfterParty.WebUI.Infrastructure
{
    public class MemoryCacheService : IMemoryCacheService
    {
        public MemoryCacheService()
        {
            
            MemoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public MemoryCache MemoryCache
        {
            get;
            set;
        }
    }
}