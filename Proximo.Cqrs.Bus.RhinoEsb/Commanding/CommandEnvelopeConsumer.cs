using Proximo.Cqrs.Core.Commanding;
using Rhino.ServiceBus;

namespace Proximo.Cqrs.Bus.RhinoEsb.Commanding
{
    public class CommandEnvelopeConsumer : ConsumerOf<CommandEnvelope>
	{
		private readonly ICommandRouter _router;

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