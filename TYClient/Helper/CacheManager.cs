using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using TY.SPIMS.Controllers;
using System.Web;
using System.Threading;

namespace TY.SPIMS.Client.Helper
{
    public class CacheManager
    {
        private static HttpRuntime _httpRuntime;

        public static Cache Cache
        {
            get
            {
                EnsureHttpRuntime();
                return HttpRuntime.Cache;
            }
        }

        private static void EnsureHttpRuntime()
        {
            if (null == _httpRuntime)
            {
                try
                {
                    Monitor.Enter(typeof(CacheManager));
                    if (null == _httpRuntime)
                    {
                        // Create an Http Content to give us access to the cache.
                        _httpRuntime = new HttpRuntime();
                    }
                }
                finally
                {
                    Monitor.Exit(typeof(CacheManager));
                }
            }
        }
        
        public static void AddItem(string name, object o)
        {
            CacheManager.Cache.Add(name, o, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default,
                (a, b, c) => { });
        }

        public static object GetItem(string name)
        {
            return Cache.Get(name);
        }

        public static void BuildInitialCache()
        {
            var purchaseController = IOC.Container.GetInstance<PurchaseController>();
            CacheManager.AddItem("Puchases", purchaseController.FetchAllPurchases());
        }
    }
}
