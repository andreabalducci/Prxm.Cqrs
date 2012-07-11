using System;
using System.Reflection;
using Castle.MicroKernel;
using CommonDomain.Persistence;
using Proximo.Cqrs.Bus.RhinoEsb.Commanding;
using Proximo.Cqrs.Server.Commanding;
using Rhino.ServiceBus;
using Sample.Commands.Inventory;
using Sample.Domain.Inventory.Domain;
using Sample.Infrastructure.Messaging;

namespace Sample.Server.Messaging
{
    public class CommandEnvelopeConsumer : ConsumerOf<CommandEnvelope>
    {
        private IKernel _kernel;

        public CommandEnvelopeConsumer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Consume(CommandEnvelope message)
        {
            var commandType = message.Command.GetType();
            var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var consumer = _kernel.Resolve(commandHandlerType);

			// we are assuming sync execution and we object tracking by the container
			// todo: change the lifestyle to a truly transient one ?
			MethodInfo mi = commandHandlerType.GetMethod("Handle", new[] { commandType });
            mi.Invoke(consumer, new object[] { message.Command });
            
            _kernel.ReleaseComponent(consumer);
        }
    }
}