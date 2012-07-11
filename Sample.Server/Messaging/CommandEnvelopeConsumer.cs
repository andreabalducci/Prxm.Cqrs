using System;
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
		private ICommandRouter _router;

		public CommandEnvelopeConsumer(ICommandRouter router)
		{
			_router = router;
		}

		public void Consume(CommandEnvelope message)
		{
			_router.RouteToHandler(message.Command);
		}
	}
}