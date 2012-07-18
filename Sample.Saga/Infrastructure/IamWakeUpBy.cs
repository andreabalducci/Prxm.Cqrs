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
	/// 
	/// the saga will then have a method whose signature accept a single type(T) argument that will be invoked by the infrastructure code
	/// to handle the saga state transitions
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IamWokeUpBy<T> where T : ICommand, IDomainEvent
	{
	}
}
