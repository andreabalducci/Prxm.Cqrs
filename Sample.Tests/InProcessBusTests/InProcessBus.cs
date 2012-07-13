using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Proximo.Cqrs.Bus.Local.Commanding;
using Sample.Domain.Inventory.Handlers;
using Proximo.Cqrs.Server.Commanding;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;
using Castle.MicroKernel;
using System.Threading;

namespace Sample.Tests.InProcessBusTests
{
	[TestFixture]
	public class InProcessBus
	{
		private static IWindsorContainer CreateContainer()
		{
			var container = new WindsorContainer();

            //// command handlers
            //container.Register(
            //    Classes
            //        .FromAssemblyContaining<TestCommandHandler>()
            //        .BasedOn(typeof(ICommandHandler<>))
            //        .WithServiceAllInterfaces()
            //        .LifestyleTransient()
            //);

			container.Register(Component.For<IDebugLogger>().ImplementedBy<DebugLogger>());

			container.Register(Component.For<ICommandQueue>().ImplementedBy<InProcessCommandQueue>());

			// env wiring
			container.Register(
				Component.For<ICommandRouter>().ImplementedBy<DefaultCommandRouter>(),
				Component.For<ICommandHandlerFactory>().ImplementedBy<CastleCommandHandlerFactory>()
			);

			return container;
		}

		private IWindsorContainer _container;

		public InProcessBus()
		{
			_container = CreateContainer();
		}

		[Test]
		public void InProcessCallTest()
		{
			var commandSender = _container.Resolve<ICommandQueue>();
			Assert.IsNotNull(commandSender);

			Guid cId = Guid.NewGuid();
			TestCommand tc = new TestCommand(cId, (c) =>
				{
					Assert.AreEqual(cId, c.Id);
				});

			commandSender.Enqueue(tc);
		}

		[Test]
		public void command_handler_should_be_able_to_enqueue()
		{
			var commandSender = _container.Resolve<ICommandQueue>();
			Assert.IsNotNull(commandSender);
			Guid cId = Guid.NewGuid();

			bool secondCommandHasBeenExecuted = false;
			bool firstCommandHasBeenExecuted = false;
			var secondCommand = new TestCommand(cId, (id) =>
														 {
															 Assert.IsTrue(firstCommandHasBeenExecuted, "Executing in the context of the first handler and not after");
															 secondCommandHasBeenExecuted = true;
														 });
			var firstCommand = new TestCommand(cId, (id) =>
														{
															commandSender.Enqueue(secondCommand);
															firstCommandHasBeenExecuted = true;
														});

			commandSender.Enqueue(firstCommand);

			Assert.IsTrue(secondCommandHasBeenExecuted);
		}

		[Test]
		public void multiple_threads_sending_commands_all_executed_on_first_calling_thread()
		{
			var commandSender = _container.Resolve<ICommandQueue>();
			Assert.IsNotNull(commandSender);

			int _counter = 0;
			int _tag = 0;
			const int maxlimit = 500; // tested with 500000

			for (int i = 0; i < maxlimit; i++)
			{
				ThreadPool.QueueUserWorkItem(o =>
				{
					Guid cId = Guid.NewGuid();
					TestCommand tc = new TestCommand(cId, (c) =>
					{
						Assert.AreEqual(cId, c.Id);
						Console.WriteLine("Command Tag: " + c.Tag);
						Console.WriteLine("Calling thread id: " + c.ThreadId);
						Console.WriteLine("Execution Thread Id: " + Thread.CurrentThread.ManagedThreadId);
						Console.WriteLine("Executed command: " + _counter);
						
						Interlocked.Increment(ref _counter);
					});
					tc.ThreadId = Thread.CurrentThread.ManagedThreadId;
					tc.Tag = _tag.ToString() + " - Caller Thread Id: " + Thread.CurrentThread.ManagedThreadId;
					Interlocked.Increment(ref _tag);
					commandSender.Enqueue(tc);
				});
			}

			while (_counter < maxlimit)
				Thread.Sleep(500);

			Assert.AreEqual(maxlimit, _counter);
		}
	}

	public class DebugLogger : IDebugLogger
	{
		public void Log(string message)
		{
			Debug.WriteLine(message);
		}
	}

	public class TestCommand : ICommand
	{
		public Guid Id { get; set; }

		public int ThreadId { get; set; }

		/// <summary>
		/// function that is called by the TestCommandHandler
		/// </summary>
		public Action<TestCommand> Callback { get; set; }

		public string Tag { get; set; }

		public TestCommand(Guid id, Action<TestCommand> callback)
		{
			Id = id;
			Callback = callback;
		}
	}

    ///// <summary>
    ///// very ugly test we pass the command id to the callback assigned to the command itself
    ///// on the test we check it's called
    ///// </summary>
    //public class TestCommandHandler : ICommandHandler<TestCommand>
    //{
    //    public void Handle(TestCommand command)
    //    {
    //        command.Callback(command);
    //    }
    //}

	public class CastleCommandHandlerFactory : ICommandHandlerFactory
	{
		private IKernel _kernel;

		public CastleCommandHandlerFactory(IKernel kernel)
		{
			_kernel = kernel;
		}

		public object CreateHandler(Type commandType)
		{
			return _kernel.Resolve(commandType);
		}

		public void ReleaseHandler(object handler)
		{
			_kernel.ReleaseComponent(handler);
		}
	}
}
