using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;


namespace TheAfterParty.WebUI.Infrastructure
{
    public interface IMemoryCacheService
    {
        MemoryCache MemoryCache
        {
            get;
            set;
        }
    }
}