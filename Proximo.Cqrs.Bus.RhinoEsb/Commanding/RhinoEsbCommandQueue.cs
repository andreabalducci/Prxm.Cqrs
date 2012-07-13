using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;
using Rhino.ServiceBus;

namespace Proximo.Cqrs.Bus.RhinoEsb.Commanding
{
    public class RhinoEsbCommandQueue : ICommandQueue
    {
        private readonly IServiceBus _serviceBus;

        public RhinoEsbCommandQueue(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }

        public void Enqueue<T>(T command) where T : class, ICommand
        {
            var envelope = new CommandEnvelope { Command = command };
            _serviceBus.Send(envelope);
        }
    }
}
