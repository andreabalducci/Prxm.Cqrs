using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;
using Proximo.Cqrs.Core.Commanding;
using Sample.Saga.Infrastructure;
using Proximo.Cqrs.Server.Commanding;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Impl.Aggregates;
using CommonDomain.Persistence;

namespace Sample.Saga
{
	/// <summary>
	/// aggregate root that raises the demoevent1
	/// </summary>
	public class DemoAggregate1 : AggregateRoot
	{
		public DemoAggregate1() { }

		public DemoAggregate1(Guid id)
		{
			Id = id;
		}

		public void DoSomething(Guid correlationId)
		{
			RaiseEvent(new DemoEvent1() { Id = correlationId });
		}

		public void Apply(DemoEvent1 @event)
		{
		}
	}

	/// <summary>
	/// aggregate root that raises the demoevent1
	/// </summary>
	public class DemoAggregate2 : AggregateRoot
	{
		public DemoAggregate2() { }

		public DemoAggregate2(Guid id)
		{
			Id = id;
		}

		public void DoSomething(Guid correlationId)
		{
			RaiseEvent(new DemoEvent2() { Id = correlationId });
		}

		public void Apply(DemoEvent2 @event)
		{
		}
	}

	public class DemoCommandHandler : ICommandHandler
	{
		private ILogger _logger;
		private IRepository _repository;

		public DemoCommandHandler(ILogger logger, IRepository repository)
		{
			_logger = logger;
			_repository = repository;
		}

		public void Handle(DemoCommand1 command)
		{
			// it's a demo let's do some nasty things: try to load the aggregate, if it does not exists
			// create it.
			//DemoAggregate1 ar = _repository.GetById<DemoAggregate1>(command.AggregateId);
			//if (ar == null)
			//    ar = new DemoAggregate1(command.AggregateId);

			_logger.Info("[DemoCommandHandler] handling 'DemoCommand1'");
			DemoAggregate1 ar = new DemoAggregate1(command.AggregateId);
			ar.DoSomething(command.CorrelatonId);
			_logger.Info("[DemoCommandHandler] 'DemoCommand1' processed");
			_repository.Save(ar, command.Id);

		}

		public void Handle(DemoCommand2 command)
		{
			_logger.Info("[DemoCommandHandler] handling 'DemoCommand2'");
			DemoAggregate2 ar = new DemoAggregate2(command.AggregateId);
			ar.DoSomething(command.CorrelatonId);
			_logger.Info("[DemoCommandHandler] 'DemoCommand2' processed");
			_repository.Save(ar, command.Id);
		}

	}

	// all 3 Id(s) are the same, they act as a correlation Id

	public class DemoEvent1 : DomainEvent
	{
		public Guid Id { get; set; }
	}

	public class DemoEvent2 : DomainEvent
	{
		public Guid Id { get; set; }
	}

	public class DemoCommand1 : ICommand
	{
		public Guid Id { get; set; }

		public Guid AggregateId { get; set; }

		public Guid CorrelatonId { get; set; }
	}

	public class DemoCommand2 : ICommand
	{
		public Guid Id { get; set; }

		public Guid AggregateId { get; set; }

		public Guid CorrelatonId { get; set; }
	}

	public class SagaCommand : ICommand
	{
		public Guid Id { get; set; }
	}

	public class DemoSagaCommandHandler : ICommandHandler
	{
		private ILogger _logger;

		public DemoSagaCommandHandler(ILogger logger)
		{
			_logger = logger;
		}

		public void Handle(SagaCommand command)
		{
			_logger.Info(string.Format("{0} id: {1} executed", command.GetType().ToString(), command.Id));
			_logger.Info(string.Format("{0} everything worked as expected. Demo completed.", command.GetType().ToString()));
		}
	}

	public class DemoSagaState : SagaState<Guid>
	{
		// additional properties goes here
		public bool Event1Arrived { get; set; }
		public bool Event2Arrived { get; set; }
	}

	/// <summary>
	/// send the SagaCommand out when both the events are arrive
	/// </summary>
	public class DemoSaga : Saga<DemoSagaState, Guid>,
		IamWokeUpBy<DemoEvent1>,
		IamWokeUpBy<DemoEvent2>
	{
		public DemoSaga(ISagaRepository<DemoSagaState, Guid> repository, ICommandQueue commandQueue, ILogger logger) :
			base(repository, commandQueue, logger)
		{ }

		public void WokeUpBy(DemoEvent1 @event)
		{
			Logger.Info("[DemoSaga] woke up by 'DemoEvent2'");
			// load the state or create a new one if empty
			State = Repository.Load(new Dictionary<string, object>() { {"Id", @event.Id }});
			if (State == null)
				State = new DemoSagaState() { Id = @event.Id };
			// update the saga internal status
			State.Event1Arrived = true;
			// send out any command and			
			// mark is as complete if possible
			TakeBusinessDecisionAnComplete();
			// if it's not completed, persist the state
			PersistIfNotCompleted();
			Logger.Info("[DemoSaga] 'DemoEvent1' processing completed");
		}

		public void WokeUpBy(DemoEvent2 @event)
		{
			Logger.Info("[DemoSaga] woke up by 'DemoEvent2'");
			// load the state or create a new one if empty
			State = Repository.Load(new Dictionary<string, object>() { { "Id", @event.Id } });
			if (State == null)
				State = new DemoSagaState() { Id = @event.Id };
			// update the saga internal status
			State.Event2Arrived = true;
			// send out any command and			
			// mark is as complete if possible
			TakeBusinessDecisionAnComplete();
			// if it's not completed, persist the state
			PersistIfNotCompleted();
			Logger.Info("[DemoSaga] 'DemoEvent2' processing completed");
		}

		public void TakeBusinessDecisionAnComplete()
		{
			Logger.Info("[DemoSaga] Evaluating business logic");
			if (State.Event1Arrived && State.Event2Arrived)
			{
				Logger.Info("[DemoSaga] both 'DemoEvent1' and 'DemoEvent2' arrived, sending out 'SagaCommand'");
				CommandQueue.Enqueue(new SagaCommand() { Id = State.Id });

				Logger.Info("[DemoSaga] marking the operation as completed");
				MarkAsCompleted();
				Logger.Info("[DemoSaga] Saga ended");
			}
			else
			{
				Logger.Info("[DemoSaga] Saga not completed yet, awaiting for more events.");
			}
		}
	}
}
