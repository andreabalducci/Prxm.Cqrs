using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Commanding;
using System.IO;
using System.Reflection;
using Proximo.Cqrs.Core.Commanding;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Fasterflect;

namespace Sample.Server.Support
{
    /// <summary>
    /// This catalog implement this strategy, it scans all types of the assembly
    /// finding all 
    /// </summary>
    public class CastleFastReflectHandlerCatalog : ICommandHandlerCatalog
    {
        private IKernel _kernel;

        /// <summary>
        /// Scans all the assemblies to find all the candidate command executors.
        /// </summary>
        public CastleFastReflectHandlerCatalog(IKernel kernel)
        {
            _kernel = kernel;
            ScanAllAssembliesInDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void ScanAllAssembliesInDirectory(String enumerationDirectory)
        {
            var files = Directory.EnumerateFiles(enumerationDirectory);
            foreach (var fileName in files)
            {
                //provare a caricare dinamicamente un assembly
                if (Path.GetExtension(fileName).EndsWith("dll"))
                {
                    try
                    {
                        var asmName = AssemblyName.GetAssemblyName(fileName);
                        Assembly dynamicAsm = Assembly.Load(asmName);

                        var executors = dynamicAsm.GetTypes()
                            .Where(t => t.IsClass && !t.IsAbstract && typeof(ICommandHandler).IsAssignableFrom(t))
                            .ToList();

                        //now each of this class could contains a method that accepts a specific ICommandType, whatever
                        //method accepts a single object that implements ICommand and returns void is a command executor.
                        //I want also this object to be resolved by castle, because it can have dependencies.
                        foreach (var executorType in executors)
                        {
                            //register this type as transient
                            _kernel.Register(Component.For(executorType).ImplementedBy(executorType).LifeStyle.Transient);

                            foreach (var minfo in executorType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                            {
                                if (minfo.ReturnType == typeof(void))
                                {
                                    var parameters = minfo.GetParameters();
                                    if (parameters.Length == 1 && typeof(ICommand).IsAssignableFrom(parameters[0].ParameterType))
                                    {
                                        var commandType = parameters[0].ParameterType;
                                        if (cachedExecutors.ContainsKey(commandType)) 
                                        {
                                            var alreadyRegisteredInvoker = cachedExecutors[commandType];
                                            String exceptionText = String.Format("Multiple handler for command {0} found: {1}.{2} and {3}. {4}",
                                                commandType.Name, alreadyRegisteredInvoker._executorType.FullName, alreadyRegisteredInvoker.MethodName,
                                                executorType.FullName, minfo.Name);
                                            throw new ApplicationException(exceptionText);
                                        }
                                        //I've found a method returning void accepting a command, for me is a command executor
                                        MethodInvoker fastReflectInvoker = minfo.DelegateForCallMethod();

                                        cachedExecutors.Add(commandType, new HandlerInfo(fastReflectInvoker, executorType, _kernel, minfo.Name));
                                    }
                                }
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

        private class HandlerInfo 
        {
            public MethodInvoker _invoker;

            public Type _executorType;

            private IKernel _kernel;

            public String MethodName { get; private set; }

            public HandlerInfo(MethodInvoker invoker, Type executorType, IKernel kernel, String methodName)
            {
                _invoker = invoker;
                _executorType = executorType;
                _kernel = kernel;
                MethodName = methodName;
            }

            public void Execute(ICommand command) 
            {

                Object executor = null;
                try
                {
                     executor = _kernel.Resolve(_executorType);
                    _invoker.Invoke(executor, new Object[] { command });
                }
                finally
                {
                    _kernel.ReleaseComponent(executor);
                }
            }


        }

        private Dictionary<Type, HandlerInfo> cachedExecutors = new Dictionary<Type, HandlerInfo>();

        public Action<ICommand> GetExecutorFor(Type commandType)
        {
            if (!cachedExecutors.ContainsKey(commandType))
            {
                throw new NotSupportedException("No command handler for " + commandType);
            }
            return cachedExecutors[commandType].Execute;
        }
    }
}
