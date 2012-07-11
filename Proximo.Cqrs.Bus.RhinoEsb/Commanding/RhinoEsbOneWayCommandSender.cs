using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Client.Commanding;
using Proximo.Cqrs.Core.Commanding;
using Rhino.ServiceBus;

namespace Proximo.Cqrs.Bus.RhinoEsb.Commanding
{
    public class RhinoEsbOneWayCommandSender : ICommandSender
    {
        private IOnewayBus _bus;

        public RhinoEsbOneWayCommandSender(IOnewayBus bus)
        {
            _bus = bus;
        }

        public void Send<T>(T command) where T : class, ICommand
        {
            var envelope = new CommandEnvelope {Command = command};
            _bus.Send(envelope);
        }
    }
}
