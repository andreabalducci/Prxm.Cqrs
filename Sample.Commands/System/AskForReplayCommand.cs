using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Commands.System
{
	/// <summary>
	/// this is a system command issued to the engine asking it to do something, it sould not be passed through the cqrs/es pipeline
	/// </summary>
	public class AskForReplayCommand : ICommand
	{
		protected AskForReplayCommand()
		{}

		public Guid Id
		{
			get;
			private set;
		}

		public AskForReplayCommand(Guid id)
		{
			Id = id;
		}
	}
}
