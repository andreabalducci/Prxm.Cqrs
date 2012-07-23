using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;
using Proximo.Cqrs.Core.Commanding;
using Sample.Saga.Infrastructure;

namespace Sample.Saga
{
	// all 3 Id(s) are the same, they act as a correlation Id

	public class DemoEvent1 : IDomainEvent
	{
		public Guid Id { get; set; }
	}

	public class DemoEvent2 : IDomainEvent
	{
		public Guid Id { get; set; }
	}

	public class SagaCommand : ICommand
	{
		public Guid Id { get; set; }
	}

	public class DemoSagaState : SagaState
	{
		public Guid Id { get; set; }

		// additional properties goes here
		public bool Event1Arrived { get; set; }
		public bool Event2Arrived { get; set; }
	}

	/// <summary>
	/// send the SagaCommand out when both the events are arrive
	/// </summary>
	public class DemoSaga : Saga<DemoSagaState>,
		IamWokeUpBy<DemoEvent1>,
		IamWokeUpBy<DemoEvent2>
	{
		public DemoSaga(ISagaRepository repository, ICommandQueue commandQueue) :
			base(repository, commandQueue)
		{ }

		public void WokeUpBy(DemoEvent1 @event)
		{
			// load the state or create a new one if empty
			State = (DemoSagaState)Repository.Load(new Dictionary<string, object>() { { "Id", @event.Id } });
			// update the saga internal status
			State.Event1Arrived = true;
			// send out any command and			
			// mark is as complete if possible
			TakeBusinessDecisionAnComplete();
			// if it's not completed, persist the state
			PersistIfNotCompleted();
		}

		public void WokeUpBy(DemoEvent2 @event)
		{
			// load the state or create a new one if empty
			State = (DemoSagaState)Repository.Load(new Dictionary<string, object>() { { "Id", @event.Id } });
			// update the saga internal status
			State.Event2Arrived = true;
			// send out any command and			
			// mark is as complete if possible
			TakeBusinessDecisionAnComplete();
			// if it's not completed, persist the state
			PersistIfNotCompleted();
		}

		public void TakeBusinessDecisionAnComplete()
		{
			if (State.Event1Arrived && State.Event2Arrived)
			{
				CommandQueue.Enqueue(new SagaCommand() { Id = State.Id });

				MarkAsCompleted();
			}
		}
	}
}
