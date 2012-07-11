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
using Sample.Infrastructure.Messaging;

namespace Sample.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();
            new OnewayRhinoServiceBusConfiguration()
                .UseCastleWindsor(container)
                .Configure();

            container.Register(Component.For<ICommandSender>().ImplementedBy<RhinoEsbCommandSender>());


            Console.WriteLine("Client started");
            var commandSender = container.Resolve<ICommandSender>();
            var id = Guid.NewGuid();
            commandSender.Send(
                new CreateInventoryItemCommand(id)
                {
                    ItemId = id,
                    Sku = "I001",
                    Description = "New Item from client"
                }
            );
            
            Console.WriteLine("Issued new Item Command");
            Console.ReadLine();


            container.Dispose();
        }
    }
}
