using System.Reflection;
using Castle.MicroKernel;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Server.Commanding;

namespace Sample.Server.Messaging
{
    public class CommandRouter : ICommandRouter
    {
        private IKernel _kernel;

        public CommandRouter(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void RouteToHandler(ICommand command)
        {
            var commandType = command.GetType();
            var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var consumer = _kernel.Resolve(commandHandlerType);

            // we are assuming sync execution and we object tracking by the container
            // todo: change the lifestyle to a truly transient one ?
            MethodInfo mi = commandHandlerType.GetMethod("Handle", new[] { commandType });
            mi.Invoke(consumer, new object[] { command });

            _kernel.ReleaseComponent(consumer);
        }
    }
}