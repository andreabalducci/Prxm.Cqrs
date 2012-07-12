using Proximo.Cqrs.Client.Commanding;
using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Bus.Local.Commanding
{
	public class InProcessCommandSender : ICommandSender
	{
		private ICommandRouter _commandRouter;

		public InProcessCommandSender(ICommandRouter commadRouter)
		{
			_commandRouter = commadRouter;
		}

		public void Send<T>(T command) where T : class, Core.Commanding.ICommand
		{
			_commandRouter.RouteToHandler(command);
		}
	}
}
