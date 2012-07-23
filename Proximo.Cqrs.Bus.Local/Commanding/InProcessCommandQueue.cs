using System;
using System.Collections.Generic;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;

namespace Proximo.Cqrs.Bus.Local.Commanding
{
	/// <summary>
	/// In the actual implementation everithing is executed in the context of the thread that enter the while cycle
	/// this can change over time if the queue gets empty and another thread start queueing commands.
	/// 
	/// this type of queue should be used just for testing purposes.
	/// 
	/// TODO: exception handling
	/// </summary>
	public class InProcessCommandQueue : ICommandQueue
	{
		private ILogger _logger;
		private readonly ICommandRouter _commandRouter;

		private readonly ConcurrentQueue<ICommand> _commands = new ConcurrentQueue<ICommand>();

		private bool _running;
		public InProcessCommandQueue(ICommandRouter commadRouter, ILogger logger)
		{
			_commandRouter = commadRouter;
			_logger = logger;
			_running = false;
		}

		public void Enqueue<T>(T command) where T : class, Core.Commanding.ICommand
		{
			_logger.Info("Queuing command " + command.GetType());
			_commands.Enqueue(command);

			if (_running)
				return;
			
			ICommand cmd;
			while (_commands.TryDequeue(out cmd))
			{
				_running = true;
				_commandRouter.RouteToHandler(cmd);
				_running = false;
			}
		}


        public void Enqueue<T>(T command, TimeSpan delay) where T : class, ICommand
        {
			// first rough implementation: to simulate the delayed delivery just enqueue the command at the given time.
			System.Threading.ThreadPool.QueueUserWorkItem(
				(o) => {
					Thread.Sleep(delay);
					Enqueue(command);
				}
				);
		}

        public void Enqueue<T>(T command, DateTime datetime) where T : class, ICommand
        {
			// first rough implementation: to simulate the delayed delivery just enqueue the command at the given time.
			TimeSpan delay = datetime.Subtract(DateTime.UtcNow);
			System.Threading.ThreadPool.QueueUserWorkItem(
				(o) =>
				{
					Thread.Sleep(delay);
					Enqueue(command);
				}
				);
        }
    }
}
