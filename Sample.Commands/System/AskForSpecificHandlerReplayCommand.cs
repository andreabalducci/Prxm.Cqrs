using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Commands.System
{
    /// <summary>
    /// TODO: is this the right way to do this?
    /// </summary>
    public class AskForSpecificHandlerReplayCommand : ICommand
    {
        public String Handlertype { get; set; }

        public Guid Id
        {
            get;
            private set;
        }

        public AskForSpecificHandlerReplayCommand(Guid id, String handlerType)
        {
            Id = id;
            Handlertype = handlerType;
        }
    }
}
