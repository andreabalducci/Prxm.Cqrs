using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Proximo.Cqrs.Bus.RhinoEsb.Commanding;
using Proximo.Cqrs.Client.Commanding;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Impl;
using Sample.Commands.Inventory;
using Proximo.Cqrs.Core.Commanding;
using log4net.Config;

namespace Sample.Client
{
	class Program
	{
		static void Main(string[] args)
		{
		    XmlConfigurator.Configure();
			//
			// setup
			//
			var container = new WindsorContainer();
			new OnewayRhinoServiceBusConfiguration()
				.UseCastleWindsor(container)
				.Configure();

			container.Register(Component.For<ICommandSender>().ImplementedBy<RhinoEsbCommandSender>());

			Console.WriteLine("Client ready");

			//
			// Create command
			//
			var id = Guid.NewGuid();
			ICommand command = new CreateInventoryItemCommand(id)
							  {
								  ItemId = id,
								  Sku = "I001",
								  Description = "New Item from client"
							  };


			//
			// Send command
			//
			var commandSender = container.Resolve<ICommandSender>();
			commandSender.Send(command);
			Console.WriteLine("Issued new Item Command");

			// 
			// create an update command
			//
			command = new UpdateInventoryItemDescriptionCommand(Guid.NewGuid())
			{
				ItemId = id,
				Description = "Updated Item description"
			};
			commandSender.Send(command);
			Console.WriteLine("Issued update Item description Command");
			//
			// shutdown
			//
			Console.ReadLine();
			container.Dispose();
		}
	}
}
