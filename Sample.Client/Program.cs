using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Impl;
using Sample.Commands.Inventory;

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
            onewayBus.Send(new CreateNewItemCommand
            {
                ItemId = Guid.NewGuid(),
                ItemCode = "I001",
                ItemDescription = "New Item from client"
            });
            Console.WriteLine("new item sent");

            Console.ReadLine();
        }
    }
}
