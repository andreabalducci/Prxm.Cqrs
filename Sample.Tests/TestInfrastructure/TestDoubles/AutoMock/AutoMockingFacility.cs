
//using AutoMockingContainerSource;
using System;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;


namespace Sample.Tests.TestInfrastructure.TestDoubles.AutoMock
{
	public class AutoMockingFacility : IFacility
	{
		private IAutoMockingRepository _relatedRepository;
		private AutoMockingDependencyResolver _autoMockingDependencyResolver;

		public AutoMockingFacility(IAutoMockingRepository relatedRepository)
		{
			_relatedRepository = relatedRepository;
		}

		#region IFacility Members
		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			_autoMockingDependencyResolver = new AutoMockingDependencyResolver(_relatedRepository);
			kernel.Resolver.AddSubResolver(_autoMockingDependencyResolver);
		}

		public void Terminate()
		{
		}
		#endregion

	}
}