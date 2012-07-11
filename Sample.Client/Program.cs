using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Impl;
using Sample.Commands.Inventory;
using Sample.Infrastructure.Commanding;

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
            var envelope = new CommandEnvelope()
                               {
                                   Command =
                                       new CreateNewItemCommand(id)
                                           {
                                               ItemId = id,
                                               ItemCode = "I001",
                                               ItemDescription = "New Item from client"
                                           }
                               };

            onewayBus.Send(envelope);
            // onewayBus.Send(envelope); -> should throw an exception server side
            Console.WriteLine("Issued new Item Command");

            Console.ReadLine();
        }
    }
}
