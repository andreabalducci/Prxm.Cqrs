using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Commands.System;
using Proximo.Cqrs.Server.Commanding;
using Proximo.Cqrs.Core.Support;

namespace Sample.Server.CommandHandlers
{
	public class AskForReplayCommandHandler : ICommandHandler<AskForReplayCommand>
	{
		public AskForReplayCommandHandler(IDebugLogger logger)
		{
			_logger = logger;
		}
		private IDebugLogger _logger;

		public void Handle(AskForReplayCommand command)
		{
			// ask the engine to perform a complete event replay
			_logger.Log("Event Replay Start");

			_logger.Log("Event Replay Completed");
		}
	}
}
