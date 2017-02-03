using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using StructureMap;

namespace MoveLikeJogger.DependencyResolution
{
    public class StructureMapScope : IDependencyScope
    {
        private readonly IContainer _container;

        public StructureMapScope(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType.IsAbstract || serviceType.IsInterface)
                return _container.TryGetInstance(serviceType);

            return _container.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances<object>().Where(s => s.GetType() == serviceType);
        }

        public void Dispose()
        {

        }
    }
}