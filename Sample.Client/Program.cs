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
using Sample.Commands.Purchases;
using log4net.Config;
using Proximo.Cqrs.Bus.RhinoEsb.Castle;
using Sample.Commands.System;

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
			
			container.Install(
				new OnewayRhinoServiceBusInstaller()
				);

			container.Register(Component.For<ICommandSender>().ImplementedBy<RhinoEsbOneWayCommandSender>());
            //
            // Send command
            //
            var commandSender = container.Resolve<ICommandSender>();

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


			commandSender.Send(command);
			Console.WriteLine("Issued new Item Command");
             
/*
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

			// ask to replay the events
			System.Threading.Thread.Sleep(3000);

			commandSender.Send(new AskForReplayCommand(Guid.NewGuid()));
			Console.WriteLine("Issued Ask For Replay Command");
*/			
            /*
			//
			// Bill of lading
			//
			RegisterBillOfLadingCommand receiveBoL = new RegisterBillOfLadingCommandBuilder(Guid.NewGuid())
				.From("Lucas Arts", "Somewhere")
				.IssuedAt(new DateTime(2012, 3, 12))
                .Numbered("001")
				    .AddRow(Guid.NewGuid(), "MI", "The Secret of Monkey Island", 1000)
				    .AddRow(Guid.NewGuid(), "ZAK", "Zak McKracken and the Alien Mindbenders", 1000)
				.Build();

			commandSender.Send(receiveBoL);
            Console.WriteLine("Received Bill of Lading");
			*/
            /*
            
			// ask to replay the events
			System.Threading.Thread.Sleep(4000);
			commandSender.Send(new AskForReplayCommand(Guid.NewGuid()));
			Console.WriteLine("Issued Ask For Replay Command");
            */
			//
			// shutdown
			//
			Console.ReadLine();
			container.Dispose();
		}
	}
}
