using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Server.Support
{
    public class CastleEventHandlerFactory : IDomainEventHandlerFactory
    {
        private readonly IKernel _kernel;

        public CastleEventHandlerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IEnumerable<object> CreateHandlers(Type eventHandlerType)
        {
            return _kernel.HasComponent(eventHandlerType) ? 
                _kernel.ResolveAll(eventHandlerType).Cast<object>().ToList() : 
                null;
        }

        public void ReleaseHandlers(IEnumerable<object> handlers)
        {
            foreach (var h in handlers)
            {
                _kernel.ReleaseComponent(h);
            }
        }
    }
}
