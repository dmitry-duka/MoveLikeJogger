using System;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;

namespace MoveLikeJogger
{
    public static class StructureMapConfigurator
    {
        public static void Configure()
        {
            var container = DependencyResolution.IoC.Initialize();
            var resolver = new DependencyResolution.StructureMapDependencyResolver(container);

            HttpRuntime.Cache.Add(typeof(IServiceProvider).FullName, resolver,
                null, Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);

            DependencyResolver.SetResolver(resolver);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }
    }
}