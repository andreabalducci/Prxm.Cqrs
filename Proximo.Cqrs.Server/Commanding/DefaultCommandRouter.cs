using System.Reflection;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;

namespace Proximo.Cqrs.Server.Commanding
{
    /// <summary>
    /// this is the default router for commands, it simply execute in process the
    /// command given an handler.
    /// </summary>
    public class DefaultCommandRouter : ICommandRouter
    {
        private ICommandHandlerCatalog _commandHandlerCatalog;
        
        private IDebugLogger _logger;

        public DefaultCommandRouter(ICommandHandlerCatalog commandHandlerCatalog, IDebugLogger logger)
        {
            _commandHandlerCatalog = commandHandlerCatalog;
            _logger = logger;
        }

        public void RouteToHandler(ICommand command)
        {
            _logger.Log("[queue] processing command " + command.ToString());

            var commandType = command.GetType();
            
            //get the executor function from the catalog, and then simply execute the command.
            var executorFunction = _commandHandlerCatalog.GetExecutorFor(commandType);
            executorFunction(command);

            //this is the old code, still working.
            //var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            //var consumer = _handlerFactory.CreateHandler(commandHandlerType);

            //// we are assuming sync execution and we object tracking by the container
            //// todo: change the lifestyle to a truly transient one ?
            //MethodInfo mi = commandHandlerType.GetMethod("Handle", new[] { commandType });
            //mi.Invoke(consumer, new object[] { command });

            _logger.Log("[queue] command handled " + command.ToString());
            _logger.Log("");
        }
    }
}