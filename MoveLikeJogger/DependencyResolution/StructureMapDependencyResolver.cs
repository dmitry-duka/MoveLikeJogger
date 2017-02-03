using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using StructureMap;

namespace MoveLikeJogger.DependencyResolution
{
    public class StructureMapDependencyResolver : System.Web.Mvc.IDependencyResolver, IDependencyResolver, IServiceProvider
    {
        private IContainer _container;

        public StructureMapDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Resolving service of type \"{serviceType.AssemblyQualifiedName}\"...", "DI IOC");

                if (serviceType.IsAbstract || serviceType.IsInterface)
                    return _container.TryGetInstance(serviceType);

                return _container.GetInstance(serviceType);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(
                    $"ERROR: Failed to resolve service of type \"{serviceType.AssemblyQualifiedName}\"...{Environment.NewLine}{ex}",
                    "DI IOC");

                throw;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Resolving services of type \"{serviceType.AssemblyQualifiedName}\"...", "DI IOC");

                return _container.GetAllInstances<object>().Where(s => s.GetType() == serviceType);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(
                    $"ERROR: Failed to resolve services of type \"{serviceType.AssemblyQualifiedName}\"...{Environment.NewLine}{ex}",
                    "DI IOC");

                throw;
            }
        }

        public IDependencyScope BeginScope()
        {
            return new StructureMapScope(_container);
        }

        public void Dispose()
        {
            if (_container == null)
            {
                return;
            }

            _container.Dispose();
            _container = null;
        }
    }
}