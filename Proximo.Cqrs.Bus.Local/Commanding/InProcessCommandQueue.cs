using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Bus.Local.Commanding
{
	public class InProcessCommandQueue : ICommandQueue
	{
		private ICommandRouter _commandRouter;

		public InProcessCommandQueue(ICommandRouter commadRouter)
		{
			_commandRouter = commadRouter;
		}

		public void Enqueue<T>(T command) where T : class, Core.Commanding.ICommand
		{
			_commandRouter.RouteToHandler(command);
		}
	}
}
