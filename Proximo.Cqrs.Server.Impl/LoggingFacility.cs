using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.Core;
using Proximo.Cqrs.Core.Support;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;

namespace Proximo.Cqrs.Server.Impl
{
    public class LoggingFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Resolver.AddSubResolver(new LoggerResolver());
        }
    }

    public class LoggerResolver : ISubDependencyResolver
    {

        public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType == typeof(ILogger);
        }

        /// <summary>
        /// It simply creates a concrete logger using the concrete type of the dependency implementation, this permits to have for a class that
        /// is called XXX a logger named XXX
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parentResolver"></param>
        /// <param name="model"></param>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
        {
            return new Log4netLogger(model.Implementation);
        }
    }
}
