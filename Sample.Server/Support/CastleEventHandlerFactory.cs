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

        public object CreateHandler(Type eventHandlerType)
        {
            if(_kernel.HasComponent(eventHandlerType))
                return _kernel.Resolve(eventHandlerType);
            return null;
        }

        public void ReleaseHandler(object handler)
        {
            _kernel.ReleaseComponent(handler);
        }
    }
}
