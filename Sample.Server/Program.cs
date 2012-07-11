using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using EventStore;
using EventStore.Serialization;
using MongoDB.Bson.Serialization;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;
using Sample.Domain.Inventory.Domain.Events;
using Sample.Domain.Inventory.Handlers;
using Sample.Infrastructure.Commanding;
using Sample.Infrastructure.Eventing;
using Sample.Infrastructure.Support;
using Sample.Server.Messaging;

namespace Sample.Server
{
    class Program
    {
        private static IWindsorContainer _container;

        static void Main(string[] args)
        {
            PrepareQueues.Prepare("msmq://localhost/cqrs.sample", QueueType.Standard);


            _container = CreateContainer();
            BootStrapper.GlobalContainer = _container;
            ConfigureMongo();

            var host = new DefaultHost();
            host.Start<BootStrapper>();

            Console.WriteLine("Server is running");
            Console.ReadLine();
        }

        private static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();

            // command handlers
            container.Register(
                Classes
                    .FromAssemblyContaining<NewInventoryItemHandler>()
                    .BasedOn(typeof(ICommandHandler<>))
                    .WithServiceAllInterfaces()
                    .LifestyleTransient()
            );

            // registers the Rhino.ServiceBus endpoints
            container.Register(
                Classes
                    .FromThisAssembly()
                    .BasedOn(typeof(ConsumerOf<>))
                    .WithServiceAllInterfaces()
                    .LifestyleTransient()
            );

            // CommonDomain & EventStore initialization 
            container.Register(
                Component.For<IConstructAggregates>().ImplementedBy<AggregateFactory>(),
                Component.For<IDetectConflicts>().ImplementedBy<ConflictDetector>().LifestyleTransient(),
                Component.For<IRepository>().ImplementedBy<EventStoreRepository>().LifestyleTransient(),
                Component.For<IStoreEvents>()
                    .UsingFactoryMethod<IStoreEvents>(k => Wireup.Init()
                                                               .UsingMongoPersistence("server", new DocumentObjectSerializer())
															   //.UsingSynchronousDispatchScheduler( ... ) // enable synchronous or asyncrhonous dispatching of domainevents
															   //.UsingAsynchronousDispatchScheduler( ... ) 
															   .InitializeStorageEngine()
                                                               .Build())
                );

            // env wiring
            container.Register(
                Component.For<IDebugLogger>().ImplementedBy<ConsoleDebugLogger>()
            );

            return container;
        }


        private static void ConfigureMongo()
        {
            var assembly = typeof(InventoryItemCreated).Assembly;
            var domainEvents = assembly.GetTypes().Where(x => typeof(IDomainEvent).IsAssignableFrom(x));

            // automapping domain events
            foreach (var domainEvent in domainEvents)
            {
                BsonClassMap.LookupClassMap(domainEvent);
            }
        }
    }

    public class ConsoleDebugLogger : IDebugLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
