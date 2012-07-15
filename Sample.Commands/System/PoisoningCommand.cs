using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Commands.System
{
    public class PoisoningCommand : ICommand
    {
        public Guid Id { get; protected set; }

        protected PoisoningCommand()
        {
        }

        public PoisoningCommand(Guid id)
        {
            Id = id;
        }
    }
}
