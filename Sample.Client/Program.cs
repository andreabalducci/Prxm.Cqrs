using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Impl;
using Sample.Commands.Inventory;
using Sample.Infrastructure.Commanding;
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


            Console.WriteLine("Client started");
            var onewayBus = container.Resolve<IOnewayBus>();
            var id = Guid.NewGuid();
            onewayBus.Send(
                new CommandEnvelope()
                {
                    Command =
                        new CreateNewItemCommand(id)
                        {
                            ItemId = id,
                            ItemCode = "I001",
                            ItemDescription = "New Item from client"
                        }
                });
            Console.WriteLine("Issued new Item Command");

            Console.ReadLine();
        }
    }
}
