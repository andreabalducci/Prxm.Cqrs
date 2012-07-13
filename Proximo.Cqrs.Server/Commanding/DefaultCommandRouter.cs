using System.Reflection;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;

namespace Proximo.Cqrs.Server.Commanding
{
    public class DefaultCommandRouter : ICommandRouter
    {
        private ICommandHandlerFactory _handlerFactory;
        private IDebugLogger _logger;

        public DefaultCommandRouter(ICommandHandlerFactory handlerFactory, IDebugLogger logger)
        {
            _handlerFactory = handlerFactory;
            _logger = logger;
        }

        public void RouteToHandler(ICommand command)
        {
            _logger.Log("[queue] processing command " + command.ToString());

            var commandType = command.GetType();
            var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var consumer = _handlerFactory.CreateHandler(commandHandlerType);

            // we are assuming sync execution and we object tracking by the container
            // todo: change the lifestyle to a truly transient one ?
            MethodInfo mi = commandHandlerType.GetMethod("Handle", new[] { commandType });
            mi.Invoke(consumer, new object[] { command });

            _handlerFactory.ReleaseHandler(consumer);
            _logger.Log("[queue] command handled " + command.ToString());
            _logger.Log("");
        }
    }
}