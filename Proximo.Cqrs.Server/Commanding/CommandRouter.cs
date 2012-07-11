using System.Reflection;
using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Server.Commanding
{
    public class CommandRouter : ICommandRouter
    {
        private ICommandHandlerFactory _handlerFactory;

        public CommandRouter(ICommandHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        public void RouteToHandler(ICommand command)
        {
            var commandType = command.GetType();
            var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var consumer = _handlerFactory.CreateHandler(commandHandlerType);

            // we are assuming sync execution and we object tracking by the container
            // todo: change the lifestyle to a truly transient one ?
            MethodInfo mi = commandHandlerType.GetMethod("Handle", new[] { commandType });
            mi.Invoke(consumer, new object[] { command });

            _handlerFactory.ReleaseHandler(consumer);
        }
    }
}