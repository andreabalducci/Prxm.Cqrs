using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Saga.Infrastructure
{
	/// <summary>
	/// Base class for a saga, it will encapsulate its persistent state in a class.
	/// 
	/// It acts as a workflow coordinator between different AR, it's responsible for sending commands
	/// to the AR in order to ask them to perform actions.
	/// 
	/// It should NOT act on them directly changing their state, it acts as a business decision point and command dispatching
	/// 
	/// the idea is this:
	/// - a saga is wake up by command or events
	/// - in the handling function the saga need to retrieve its state from a repository (we need a common correlation id in all the messages handled in order to uniquely identify the saga that is executing)
	/// - the saga takes its decisions and sends commands to tell the world what to do
	/// - if it's completed the saga mark itself as completed (and possibly remove itself from the storage...no need to keep these data right ?!?!)
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	public abstract class Saga<TState> where TState : SagaState
	{
		protected TState State { get; set; }

		protected ICommandQueue CommandQueue { get; set; }

		public Saga(ICommandQueue commandQueue)
		{
			CommandQueue = commandQueue;
		}
	}
}
