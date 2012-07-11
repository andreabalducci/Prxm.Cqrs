using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Serialization;
using MongoDB.Bson.Serialization;
using Proximo.Cqrs.Bus.RhinoEsb.Commanding;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Commanding;
using Proximo.Cqrs.Server.Eventing;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;
using Sample.Domain.Inventory.Domain.Events;
using Sample.Domain.Inventory.EventHandlers;
using Sample.Domain.Inventory.Handlers;
using Sample.Server.Messaging;
using Sample.Server.Support;
using log4net.Config;

namespace Sample.Server
{
    class Program
    {
        private static IWindsorContainer _container;

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

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

            // event handlers
            container.Register(
                Classes
                    .FromAssemblyContaining<NewInventoryItemCreatedEventHandler>()
                    .BasedOn(typeof(IDomainEventHandler<>))
                    .WithServiceAllInterfaces()
                    .LifestyleTransient()
            );

            // registers the Rhino.ServiceBus endpoints
            container.Register(
                Classes
                    .FromAssemblyContaining<CommandEnvelopeConsumer>()
                    .BasedOn(typeof(ConsumerOf<>))
                    .WithServiceAllInterfaces()
                    .LifestyleTransient()
            );

            // env wiring
            container.Register(
                Component.For<IDebugLogger>().ImplementedBy<ConsoleDebugLogger>(),
                
                // commands
                Component.For<ICommandRouter>().ImplementedBy<CommandRouter>(),
                Component.For<ICommandHandlerFactory>().ImplementedBy<CastleCommandHandlerFactory>(),
                
                // events
                Component.For<IDomainEventHandlerFactory>().ImplementedBy<CastleEventHandlerFactory>(),
                Component.For<IDomainEventRouter>().ImplementedBy<DefaultDomainEventRouter>(),
                Component.For<IDispatchCommits>().ImplementedBy<CommitToEventsDispatcher>()
            );

            // CommonDomain & EventStore initialization 
            container.Register(
                Component.For<IConstructAggregates>().ImplementedBy<AggregateFactory>(),
                Component.For<IDetectConflicts>().ImplementedBy<ConflictDetector>().LifestyleTransient(),
                Component.For<IRepository>().ImplementedBy<EventStoreRepository>().LifestyleTransient(),
                Component.For<IStoreEvents>()
                    .UsingFactoryMethod<IStoreEvents>(k => Wireup.Init()
                                                               .UsingMongoPersistence("server", new DocumentObjectSerializer())
                                                               .InitializeStorageEngine()
                                                               .UsingSynchronousDispatchScheduler() // enable synchronous dispatching of domainevents
                                                                    .DispatchTo(container.Resolve<IDispatchCommits>())
                                                               .Build())
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
}
