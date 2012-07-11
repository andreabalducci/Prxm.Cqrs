using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Eventing
{
    public interface IDomainEventRouter
    {
        void Dispatch(object @event);
    }
}
