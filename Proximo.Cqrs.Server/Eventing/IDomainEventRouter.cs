using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Eventing
{
    public interface IDomainEventRouter
    {
        /// <summary>
        /// TODO: This should accepts an IDomainEvent not an object
        /// </summary>
        /// <param name="event"></param>
        void Dispatch(object @event);
    }
}
