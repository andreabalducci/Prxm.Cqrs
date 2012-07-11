using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Sample.Domain.Inventory.Handlers;
using Proximo.Cqrs.Server.Commanding;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;
using Castle.MicroKernel;
using Proximo.Cqrs.Client.Commanding;
using Proximo.Cqrs.Bus.InProcess.Commanding;

namespace Sample.Tests.InProcessBusTests
{
	[TestFixture]
	public class InProcessBus
	{
		private static IWindsorContainer CreateContainer()
		{
			var container = new WindsorContainer();

			// command handlers
			container.Register(
				Classes
					.FromAssemblyContaining<TestCommandHandler>()
					.BasedOn(typeof(ICommandHandler<>))
					.WithServiceAllInterfaces()
					.LifestyleTransient()
			);

			container.Register(Component.For<ICommandSender>().ImplementedBy<InProcessCommandSender>());

			// env wiring
			container.Register(
				Component.For<ICommandRouter>().ImplementedBy<CommandRouter>(),
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
			var commandSender = _container.Resolve<ICommandSender>();
			Assert.IsNotNull(commandSender);

			Guid cId = Guid.NewGuid();
			TestCommand tc = new TestCommand(cId, (id) => 
				{
					Assert.AreEqual(cId, id);
				});

			commandSender.Send(tc);
		}
	}

	public class TestCommand : ICommand
	{
		public Guid Id { get; set; }

		public Action<Guid> Callback { get; set; }

		public TestCommand(Guid id, Action<Guid> callback)
		{
			Id = id;
			Callback = callback;
		}		
	}

	/// <summary>
	/// very ugly test we pass the command id to the callback assigned to the command itself
	/// on the test we check it's called
	/// </summary>
	public class TestCommandHandler : ICommandHandler<TestCommand>
	{
		public void Handle(TestCommand command)
		{
			command.Callback(command.Id);
		}
	}

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
