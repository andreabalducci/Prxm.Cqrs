using System;
using System.Reflection;
using Castle.MicroKernel;
using CommonDomain.Persistence;
using Rhino.ServiceBus;
using Sample.Commands.Inventory;
using Sample.Domain.Inventory.Domain;
using Sample.Infrastructure.Commanding;

namespace Sample.Server
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
            var commandHandlerType = typeof(IHandler<>).MakeGenericType(commandType);
            var consumer = _kernel.Resolve(commandHandlerType);

            MethodInfo miTypeDef = commandHandlerType.GetMethod("Handle", new[] { commandType });
            miTypeDef.Invoke(consumer, new object[] { message.Command });
            
            _kernel.ReleaseComponent(consumer);
        }
    }
}