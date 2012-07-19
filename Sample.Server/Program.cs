using System;
using System.Configuration;
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
using MongoDB.Driver;
using Proximo.Cqrs.Bus.RhinoEsb;
using Proximo.Cqrs.Bus.RhinoEsb.Commanding;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Aggregates;
using Proximo.Cqrs.Server.Commanding;
using Proximo.Cqrs.Server.Eventing;
using Proximo.Cqrs.Server.Impl;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Msmq;
using Sample.Domain.Inventory.Domain.Events;
using Sample.Server.Messaging;
using Sample.Server.Support;
using log4net.Config;
using Proximo.Cqrs.Bus.RhinoEsb.Castle;
using Sample.Server.CommandHandlers;
using Sample.QueryModel.Rebuilder;
using Sample.QueryModel.Builder;

namespace Sample.Server
{
	class Program
	{
		private static IWindsorContainer _container;

		static void Main(string[] args)
		{App_Start.NHibernateProfilerBootstrapper.PreStart();

			XmlConfigurator.Configure();
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();


			PrepareQueues.Prepare("msmq://localhost/cqrs.sample", QueueType.Standard);

			_container = CreateContainer();

			ConfigureCommandSender();
			AutomapEventsForMongoDB();
			ConfigureQueryModelBuilder();
			ConfigureQueryModelRebuilder();
            ConfigureDomainEventHandlerInitializers();

			_container.Install(
				new RhinoServiceBusInstaller()
				);

			// rebuild the views if needed (it must be done before the bus starts)
			// todo: add some tracing
			DenormalizerRebuilder rebuilder = _container.Resolve<DenormalizerRebuilder>();
			rebuilder.Rebuild();
			_container.Release(rebuilder);

            //GM: Now initialization of the handler and discovering of specific support
            //This can be moved to the core if this concept of initialization is good.


            InitializeDomainEventsHandlers();

			_container.Resolve<IStartableBus>().Start();

			IStoreEvents store = _container.Resolve<IStoreEvents>();

			Console.WriteLine("Server is running");
			Console.ReadLine();
		}

        /// <summary>
        /// this is a little bit ugly, this code should me moved inside the catalog handler, for
        /// each type discovered the catalog can call initializer.
        /// Actually it simply gets the list of the event handler found on the system, creates each one
        /// with castle and then resolve and call all the domain event handler initializer that were registered
        /// on the system to initialize the handler.
        /// </summary>
        private static void InitializeDomainEventsHandlers()
        {
            IDomainEventHandlerCatalog catalog = _container.Resolve<IDomainEventHandlerCatalog>();
            IRawEventStore rawEventStore = _container.Resolve<IRawEventStore>();
            //takes all invoker
            var allHandlers = catalog.GetAllHandlers();
            var allInitializer = _container.ResolveAll<IDomainEventHandlerInitializer>();
            //cycle on all defining types, we are interested in only
            foreach (var handlerType in allHandlers)
            {
                Object realHandler = _container.Resolve(handlerType);
                foreach (var initializer in allInitializer)
                {
                    initializer.Initialize(realHandler);
                }
                _container.Release(realHandler);
            }
            _container.Release(catalog);
            _container.Release(rawEventStore);
            foreach (var initializer in allInitializer)
            {
                _container.Release(initializer);
            }
        }

		private static IWindsorContainer CreateContainer()
		{
			var container = new WindsorContainer();

			//// command handlers
			//container.Register(
			//    Classes
			//        .FromAssemblyContaining<NewInventoryItemHandler>()
			//        .BasedOn(typeof(ICommandHandler<>))
			//        .WithServiceAllInterfaces()
			//        .LifestyleTransient()
			//);

			//// No more needed, there is the catalog.
			//// wire up some system commands
			//container.Register(
			//    Classes
			//        .FromAssemblyContaining<AskForReplayCommandHandler>()
			//        .BasedOn(typeof(ICommandHandler<>))
			//        .WithServiceAllInterfaces()
			//        .LifestyleTransient()
			//);

			//// event handlers
			//container.Register(
			//    Classes
			//        .FromAssemblyContaining<NewInventoryItemCreatedEventHandler>()
			//        .BasedOn(typeof(IDomainEventHandler<>))
			//        .WithServiceAllInterfaces()
			//        .LifestyleTransient()
			//);

			//register the custom logging facility
			container.AddFacility<LoggingFacility>();

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
				Component.For<ILogger>().ImplementedBy<Log4netLogger>(),

				// commands
				Component.For<ICommandRouter>().ImplementedBy<DefaultCommandRouter>(),
				//Component.For<ICommandHandlerFactory>().ImplementedBy<CastleCommandHandlerFactory>(),
				Component.For<ICommandHandlerCatalog, IDomainEventHandlerCatalog>().ImplementedBy<CastleFastReflectHandlerCatalog>(),
				// events
				//Component.For<IDomainEventHandlerFactory>().ImplementedBy<CastleEventHandlerFactory>(),

				Component.For<IDomainEventRouter>().ImplementedBy<DefaultDomainEventRouter>(),
				Component.For<IDomainEventRouterForQueryModelRebuild>().ImplementedBy<DomainEventRouterForQueryModelRebuild>(),
				Component.For<IDispatchCommits>().ImplementedBy<CommitToEventsDispatcher>()
			);

            //Custom interceptor of events
            container.Register(
                Component.For<IRawEventStore>().ImplementedBy<MongoRawEventStore>(),
                Component.For<IPipelineHook>().ImplementedBy<EventDispatcherInterceptor>());

			// CommonDomain & EventStore initialization 
			container.Register(
				Component.For<IConstructAggregates>().ImplementedBy<AggregateFactory>(),
				Component.For<IDetectConflicts>().ImplementedBy<ConflictDetector>().LifestyleTransient(),
				Component.For<IRepository>().ImplementedBy<EventStoreRepository>().LifestyleTransient(),
				Component.For<IStoreEvents>()
					.UsingFactoryMethod<IStoreEvents>(k => Wireup.Init()
															   .UsingMongoPersistence("server", new DocumentObjectSerializer())
															   .InitializeStorageEngine()
                                                               .HookIntoPipelineUsing(container.ResolveAll<IPipelineHook>())
															   .UsingAsynchronousDispatchScheduler()
						//.UsingSynchronousDispatchScheduler() // enable synchronous dispatching of domainevents
																	.DispatchTo(container.Resolve<IDispatchCommits>())
                                                                    
															   .Build())
				);

			return container;
		}


		private static void AutomapEventsForMongoDB()
		{
			var assembly = typeof(InventoryItemCreated).Assembly;
			var domainEvents = assembly.GetTypes().Where(x => typeof(IDomainEvent).IsAssignableFrom(x));

			BsonClassMap.RegisterClassMap<AggregateVersion>(
				map =>
				{
					map.MapIdProperty(x => x.Id).SetElementName("a");
					map.MapProperty(x => x.Version).SetElementName("v");
				}
			);


			// automapping domain events
			foreach (var domainEvent in domainEvents)
			{
				BsonClassMap.LookupClassMap(domainEvent);
			}
		}

		private static void ConfigureQueryModelBuilder()
		{
			_container.Register(
				//Classes.FromAssemblyContaining<InventoryItemDenormalizer>()
				//    .BasedOn(typeof(IDomainEventHandler<>))
				//    .WithServiceAllInterfaces()
				//    .LifestyleTransient(),
				Component.For<MongoDatabase>().UsingFactoryMethod(k =>
				{
					var builder = new MongoConnectionStringBuilder(ConfigurationManager.ConnectionStrings["query"].ToString());
					var db = MongoServer.Create(builder).GetDatabase(builder.DatabaseName);
					return db;
				}),
				Component.For(typeof(IModelWriter<>)).ImplementedBy(typeof(ModelWriter<>)).LifeStyle.Transient
			);
		}

		private static void ConfigureQueryModelRebuilder()
		{
			_container.Register(
				Component.For<IHashcodeGenerator>().ImplementedBy<HashcodeGenerator>(),
				Component.For<IDenormalizerCatalog>().ImplementedBy<DenormalizersDemoCatalog>(),
				Component.For<IDenormalizersHashesStore>().ImplementedBy<MongoDbDenormalizersHashesStore>(),
				Component.For<DenormalizerRebuilder>()
			);
		}

        private static void ConfigureDomainEventHandlerInitializers()
        {
            _container.Register(
                Component.For<IDomainEventHandlerInitializer>().ImplementedBy<ReplayableDomainEventHandlerInitializer>()
            );
        }

		private static void ConfigureCommandSender()
		{
			_container.Register(Component.For<ICommandQueue>().ImplementedBy<RhinoEsbCommandQueue>());
		}
	}
}

