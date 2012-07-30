using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;

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
	/// - a saga is wake up by an event (even commands in the future): the domain expresses the will to do something with an event that triggers the saga.
	/// - in the handling function the saga need to retrieve its state from a repository (we need a common correlation id in all the messages handled in order to uniquely identify the saga that is executing)
	/// - the saga takes its decisions and sends commands to tell the world what to do
	/// - if it's completed the saga mark itself as completed (and possibly remove itself from the storage...no need to keep these data right ?!?!)
	/// 
	/// assumption: once the saga is completed it's useless (the process is done and all the info are traced by the domain events), so we remove it form the repository
	/// 
	/// todo: add timeout support maybe using delayed messages/signals.
	/// 
	/// what if a timeout message arrives and the saga was completed? we risk to execute the saga again (if it's woke up by timeout messages)
	/// or if a timeout message arrives and the Saga is not found in the repository we can just consider it as completed and
	/// discard the message
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	public abstract class Saga<TState, TId> where TState : SagaState<TId>, new()
	{
		protected TState State { get; set; }

		protected ICommandQueue CommandQueue { get; set; }

		protected ISagaRepository<TState, TId> Repository { get; set; }

		protected ILogger Logger { get; set; }

		public Saga(ISagaRepository<TState, TId> repository, ICommandQueue commandQueue, ILogger logger)
		{
			CommandQueue = commandQueue;
			Repository = repository;
			Logger = logger;
		}

		protected void PersistIfNotCompleted()
		{
			if (!State.Completed)
			{
				Repository.Save(State);
			}
		}

		protected virtual void MarkAsCompleted()
		{
			State.Completed = true;
			// and now what ? remove the state from the database ? is this true, isn't it better to mark it as completed
			// otherwise we risk to execute the saga again (if it's woke up by timeout messages)
			// or if a timeout message arrives and the Saga is not found in the repository we can just consider it as completed and
			// discard the message
			Repository.Remove(State);
		}
	}
}
