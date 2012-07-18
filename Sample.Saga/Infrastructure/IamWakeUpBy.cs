using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Saga.Infrastructure
{
	/// <summary>
	/// marker interface that is used to specify which commands or messages are used to wakeup the saga
	/// 
	/// todo: maybe it's better to have a generic 'message' because the saga can be wake up even by 'system events'
	/// todo: we can extend this to let the saga be woke up by commands too
	/// 
	/// the saga will then have a method whose signature accept a single type(T) argument that will be invoked by the infrastructure code
	/// to handle the saga state transitions
	/// 
	/// This interface is not really needed in our actual implementation, a saga at this point can just be an IDomainEventHandler.
	/// the interface is here just to make things explicit.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IamWokeUpBy<T> : IDomainEventHandler where T : IDomainEvent
	{
		/// <summary>
		/// this is not strictly needed by our actual implementation it's here to just make things explicit
		/// </summary>
		/// <param name="event"></param>
		void WokeUpBy(T @event);
	}
}
