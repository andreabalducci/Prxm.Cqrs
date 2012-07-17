using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Fasterflect;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Commanding;
using Proximo.Cqrs.Server.Eventing;

namespace Proximo.Cqrs.Server.Impl
{
    /// <summary>
    /// This catalog implement this strategy, it scans all types of the assembly
    /// finding all 
    /// </summary>
    public class CastleFastReflectHandlerCatalog : ICommandHandlerCatalog, IDomainEventHandlerCatalog
    {
        private IKernel _kernel;

        private ILogger _logger;

        private Type[] _domainEventHandlers;

        private Type[] _commandExecutorTypes;

        /// <summary>
        /// Scans all the assemblies to find all the candidate command executors.
        /// </summary>
        public CastleFastReflectHandlerCatalog(IKernel kernel, ILogger logger)
        {
            _kernel = kernel;
            _logger = logger;
            ScanAllAssembliesInDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void ScanAllAssembliesInDirectory(String enumerationDirectory)
        {
            var files = Directory.EnumerateFiles(enumerationDirectory);
            foreach (var fileName in files)
            {
                //provare a caricare dinamicamente un assembly
                if (Path.GetExtension(fileName).EndsWith("dll") || Path.GetExtension(fileName).EndsWith("exe"))
                {
                    try
                    {
                        var asmName = AssemblyName.GetAssemblyName(fileName);

                        Assembly dynamicAsm = Assembly.Load(asmName);
                        Type[] allAssemblyTypes = null;
                        try
                        {
                            allAssemblyTypes = dynamicAsm.GetTypes();

                        }
                        catch (ReflectionTypeLoadException rtl)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var singleLoadException in rtl.LoaderExceptions)
                            {
                                sb.AppendLine(singleLoadException.Message);
                            }
                            _logger.Error("Unable to scan asssembly " + asmName.Name + " reason:\n" + sb.ToString());
                            continue;
                            //throw new ApplicationException("CastleFastReflectHandlerCatalog is unable to scan type of assembly " + asmName + "\n" + sb.ToString());
                        }

                        //convert to array to avoid risk of using list and outer modification.
                        _commandExecutorTypes = ScanForCommandExecutors(allAssemblyTypes).ToArray();
                        _domainEventHandlers = ScanForDomainEventHandler(allAssemblyTypes).ToArray();
                        foreach (var type in _commandExecutorTypes.Union(_domainEventHandlers))
                        {
                            EventHandlerDescriptionAttribute ehda;
                            if (type.IsDefined(typeof(EventHandlerDescriptionAttribute), false))
                            {
                                ehda = (EventHandlerDescriptionAttribute)type.GetCustomAttributes(
                                    typeof(EventHandlerDescriptionAttribute), false)
                                    .OfType<EventHandlerDescriptionAttribute>()
                                    .Single();
                            }
                            else
                            {
                                ehda = new EventHandlerDescriptionAttribute(); //Default values
                            }
                            if (ehda.IsSingleton)
                            {
                                _kernel.Register(Component.For(type).ImplementedBy(type).LifeStyle.Singleton);
                            }
                            else
                            {
                                _kernel.Register(Component.For(type).ImplementedBy(type).LifeStyle.Transient);
                            }
                            
                        }
                    }
                    catch (TypeLoadException ex)
                    {
                        //Create a log that tells what is wrong with that type
                        throw;
                    }
                }
            }

        }

        private List<Type> ScanForCommandExecutors(Type[] allAssemblyTypes)
        {
            //now each of this class could contains a method that accepts a specific ICommandType, whatever
            //method accepts a single object that implements ICommand and returns void is a command executor.
            //I want also this object to be resolved by castle, because it can have dependencies.
            var executors = allAssemblyTypes
                              .Where(t => t.IsClass && !t.IsAbstract && typeof(ICommandHandler).IsAssignableFrom(t))
                              .ToList();
            foreach (var executorType in executors)
            {

                ParameterInfo[] parameters = null;
                foreach (var minfo in executorType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(mi => mi.ReturnType == typeof(void) &&
                                    (parameters = mi.GetParameters()).Length == 1 &&
                                    typeof(ICommand).IsAssignableFrom(parameters[0].ParameterType)))
                {

                    var commandType = parameters[0].ParameterType;
                    if (cachedExecutors.ContainsKey(commandType))
                    {
                        var alreadyRegisteredInvoker = cachedExecutors[commandType];
                        String exceptionText = String.Format("Multiple handler for command {0} found: {1}.{2} and {3}. {4}",
                            commandType.Name, alreadyRegisteredInvoker.ExecutorType.FullName, alreadyRegisteredInvoker.MethodName,
                            executorType.FullName, minfo.Name);
                        throw new ApplicationException(exceptionText);
                    }
                    //I've found a method returning void accepting a command, for me is a command executor
                    MethodInvoker fastReflectInvoker = minfo.DelegateForCallMethod();
                    cachedExecutors.Add(commandType, new CommandExecutorInfo(fastReflectInvoker, executorType, _kernel, minfo.Name));
                }
            }
            return executors;
        }

        private List<Type> ScanForDomainEventHandler(Type[] allAssemblyTypes)
        {
            //now each of this class could contains a method that accepts a specific ICommandType, whatever
            //method accepts a single object that implements ICommand and returns void is a command executor.
            //I want also this object to be resolved by castle, because it can have dependencies.
            var handlers = allAssemblyTypes
                              .Where(t => t.IsClass && !t.IsAbstract && typeof(IDomainEventHandler).IsAssignableFrom(t))
                              .ToList();
            foreach (var eventHandlerType in handlers)
            {

                ParameterInfo[] parameters = null;
                foreach (var minfo in eventHandlerType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(mi => mi.ReturnType == typeof(void) &&
                                    (parameters = mi.GetParameters()).Length == 1 &&
                                    typeof(IDomainEvent).IsAssignableFrom(parameters[0].ParameterType)))
                {
                    var eventType = parameters[0].ParameterType;

                    //I've found a method returning void accepting a Domain Event it is an handler
                    MethodInvoker fastReflectInvoker = minfo.DelegateForCallMethod();
                    cachedHandlers.Add(new DomainEventHandlerInfo(fastReflectInvoker, eventHandlerType, eventType, _kernel, minfo.Name));
                }
            }
            return handlers;
        }

        private class CommandExecutorInfo
        {
            private MethodInvoker _invoker;

            public Type ExecutorType { get; private set; }

            private IKernel _kernel;

            public String MethodName { get; private set; }

            public CommandExecutorInfo(MethodInvoker invoker, Type executorType, IKernel kernel, String methodName)
            {
                _invoker = invoker;
                _kernel = kernel;
                ExecutorType = executorType;
                MethodName = methodName;
            }

            public void Execute(ICommand command)
            {

                Object executor = null;
                try
                {
                    executor = _kernel.Resolve(ExecutorType);
                    _invoker.Invoke(executor, new Object[] { command });
                }
                finally
                {
                    _kernel.ReleaseComponent(executor);
                }
            }


        }

        private Dictionary<Type, CommandExecutorInfo> cachedExecutors = new Dictionary<Type, CommandExecutorInfo>();

        public Action<ICommand> GetExecutorFor(Type commandType)
        {
            if (!cachedExecutors.ContainsKey(commandType))
            {
                throw new NotSupportedException("No command handler for " + commandType);
            }
            return cachedExecutors[commandType].Execute;
        }


        /// <summary>
        /// value holder for the command handler info.
        /// </summary>
        private class DomainEventHandlerInfo
        {
            public MethodInvoker _invoker;

            public Type ExecutorType { get; private set; }

            private IKernel _kernel;

            public Type EventType { get; private set; }

            public String MethodName { get; private set; }

            public DomainEventHandlerInfo(MethodInvoker invoker, Type executorType, Type eventType, IKernel kernel, String methodName)
            {
                _invoker = invoker;
                ExecutorType = executorType;
                EventType = eventType;
                _kernel = kernel;
                MethodName = methodName;
            }

            public Boolean CanHandleEvent(Type domainEventType)
            {
                return EventType.IsAssignableFrom(domainEventType);
            }

            public void Execute(IDomainEvent @event)
            
{
                Object executor = null;
                try
                {
                    executor = _kernel.Resolve(ExecutorType);
                    _invoker.Invoke(executor, new Object[] { @event });
                }
                finally
                {
                    _kernel.ReleaseComponent(executor);
                }
            }


        }

        private List<DomainEventHandlerInfo> cachedHandlers = new List<DomainEventHandlerInfo>();

        public IEnumerable<DomainEventInvoker> GetAllHandlerFor(Type domainEventType)
        {
            //TODO: Cache this
            return cachedHandlers
                .Where(h => h.CanHandleEvent(domainEventType))
                .Select<DomainEventHandlerInfo, DomainEventInvoker>(h => new DomainEventInvoker(h.Execute, h.ExecutorType));
        }

        public IDictionary<Type, Action<IDomainEvent>> GetAllHandlerForSpecificHandlertype(Type handlerType)
        {
            Dictionary<Type, Action<IDomainEvent>> retValue = new Dictionary<Type, Action<IDomainEvent>>();
            foreach (var item in cachedHandlers
                .Where(h => h.ExecutorType == handlerType))
            {
                retValue.Add(item.EventType, item.Execute);
            }
            return retValue;
        }



    }
}
