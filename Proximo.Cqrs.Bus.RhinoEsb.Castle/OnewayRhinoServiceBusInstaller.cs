using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus;

namespace Proximo.Cqrs.Bus.RhinoEsb.Castle
{
	public class OnewayRhinoServiceBusInstaller : IWindsorInstaller
	{
		#region IWindsorInstaller Members

		public void Install(global::Castle.Windsor.IWindsorContainer container, global::Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
		{
			new OnewayRhinoServiceBusConfiguration()
				.UseCastleWindsor(container)
				.Configure();
		}

		#endregion
	}
}
