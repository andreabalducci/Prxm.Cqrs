using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Eventing
{
	/// <summary>
	/// a class that holds some detailed information about the functions that are used to handle 
	/// </summary>
	public class DomainEventInvoker
	{
		// the actual function that have to be invoked in order to handle the event
		public Action<IDomainEvent> Invoker { get; set; }

		/// <summary>
		/// the type in which the onvoker is defined
		/// </summary>
		public Type DefiningType { get; set; }

        /// <summary>
        /// The type of event handled by this invoker.
        /// </summary>
        public Type HandledType { get; set; }

		public DomainEventInvoker(Action<IDomainEvent> invoker, Type definingType, Type handledType)
		{
			Invoker = invoker;
			DefiningType = definingType;
            HandledType = handledType;
		}

		public void Invoke(IDomainEvent @event)
		{
			Invoker.Invoke(@event);
		}
	}
}
