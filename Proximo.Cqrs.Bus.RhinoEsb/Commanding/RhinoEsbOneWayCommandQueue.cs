using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;
using Rhino.ServiceBus;

namespace Proximo.Cqrs.Bus.RhinoEsb.Commanding
{
    public class RhinoEsbOneWayCommandQueue : ICommandQueue
    {
        private IOnewayBus _bus;

        public RhinoEsbOneWayCommandQueue(IOnewayBus bus)
        {
            _bus = bus;
        }

        public void Enqueue<T>(T command) where T : class, ICommand
        {
            var envelope = new CommandEnvelope {Command = command};
            _bus.Send(envelope);
        }

        public void Enqueue<T>(T command, TimeSpan delay) where T : class, ICommand
        {
            throw new NotSupportedException("delayed commands not supported for OneWay bus");
        }

        public void Enqueue<T>(T command, DateTime datetime) where T : class, ICommand
        {
            throw new NotSupportedException("delayed commands not supported for OneWay bus");
        }
    }
}
