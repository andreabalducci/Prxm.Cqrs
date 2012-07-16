using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Bus.RhinoEsb.Castle;
using Proximo.Cqrs.Bus.RhinoEsb.Commanding;
using Proximo.Cqrs.Core.Commanding;
using Castle.Windsor;
using Castle.MicroKernel.Registration;

namespace Sample.Client.Wpf
{
    public class Infrastructure
    {
        private static Infrastructure _instance;
        public static Infrastructure Instance { get { return _instance; } }

        ICommandQueue commandSender;
        public Infrastructure()
        {
            var container = new WindsorContainer();

            container.Install(
                new OnewayRhinoServiceBusInstaller()
                );

            container.Register(Component.For<ICommandQueue>().ImplementedBy<RhinoEsbOneWayCommandQueue>());
            //
            // Enqueue command
            //
            commandSender = container.Resolve<ICommandQueue>();
        }

        public void SendCommand(ICommand command)
        {

            commandSender.Enqueue(command);
        }
    }
}
