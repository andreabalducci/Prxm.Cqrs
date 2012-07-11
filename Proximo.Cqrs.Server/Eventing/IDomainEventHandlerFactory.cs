using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Eventing
{
    public interface IDomainEventHandlerFactory
    {
        object CreateHandler(Type eventHandlerType);
        void ReleaseHandler(object handler);
    }
}
