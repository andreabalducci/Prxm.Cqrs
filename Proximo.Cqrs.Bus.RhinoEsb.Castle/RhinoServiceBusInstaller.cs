using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus;
using Proximo.Cqrs.Core.Bus;
using Castle.Windsor;

namespace Proximo.Cqrs.Bus.RhinoEsb.Castle
{
	public class RhinoServiceBusInstaller : IWindsorInstaller
	{
		#region IWindsorInstaller Members

		public void Install(global::Castle.Windsor.IWindsorContainer container, global::Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
		{
			new RhinoServiceBusConfiguration()
				.UseCastleWindsor(container)
				.Configure();
			
			// castle is not able to resolve references right now, we need another support interface to delay the bust start outside the installer calls
			//container.Resolve<IStartableServiceBus>().Start();
			container.Register(Component.For<IStartableBus>().ImplementedBy<Startable>().LifeStyle.Transient);
		}

		#endregion
	}

	internal class Startable : IStartableBus
	{
		private IStartableServiceBus _startableBus;

		public Startable(IStartableServiceBus startableBus)
		{
			_startableBus = startableBus;
		}

		#region IStartableBus Members

		public void Start()
		{
			_startableBus.Start();
		}

		#endregion
	}
}
