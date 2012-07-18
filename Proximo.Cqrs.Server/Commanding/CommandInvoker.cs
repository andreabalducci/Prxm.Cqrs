using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Server.Commanding
{
    /// <summary>
    /// This class is analogous to the <see cref="DomainEventInvoker"/> and is used to 
    /// encapsulate all details about command invoker discovered by the catalog.
    /// </summary>
    public class CommandInvoker
    {
        // the actual function that have to be invoked in order to handle the event
		public Action<ICommand> Invoker;

		/// <summary>
		/// the type in which the onvoker is defined
		/// </summary>
		public Type DefiningType;

        public CommandInvoker(Action<ICommand> invoker, Type definingType)
		{
			Invoker = invoker;
			DefiningType = definingType;
		}

        public void Invoke(ICommand @event)
		{
			Invoker.Invoke(@event);
		}
    }
}
