using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Server.Core
{
    /// <summary>
    /// Contains information about executed command.
    /// </summary>
    public class ExecutedCommand
    {
        public Guid Id { get; set; }

        /// <summary>
        /// the real command object that gets executed.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// True if command succeeded, false if something went wrong, ex: exception
        /// </summary>
        public Boolean IsSuccess { get; set; }

        /// <summary>
        /// Detail on what went wrong. We use this to store Exception detail or some
        /// other detail we found useful to store with the command.
        /// </summary>
        public String Error { get; set; }
    }
}
