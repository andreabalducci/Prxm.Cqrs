using System;
using System.Collections.Generic;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;

namespace Proximo.Cqrs.Bus.Local.Commanding
{
	// TODO: exception handling
	public class InProcessCommandQueue : ICommandQueue
	{
	    private IDebugLogger _logger;
        private readonly ICommandRouter _commandRouter;
		private readonly Queue<ICommand> _commands = new Queue<ICommand>();
	    private bool _running;
		public InProcessCommandQueue(ICommandRouter commadRouter, IDebugLogger logger)
		{
		    _commandRouter = commadRouter;
		    _logger = logger;
		    _running = false;
		}

	    public void Enqueue<T>(T command) where T : class, Core.Commanding.ICommand
		{
            _logger.Log("Queuing command " + command.GetType());
            _commands.Enqueue(command);

            if (_running)
                return;

            while(_commands.Count > 0)
            {
                var cmd = _commands.Dequeue();
                _running = true;
                    _commandRouter.RouteToHandler(cmd);
                _running = false;
            }
		}
	}
}
