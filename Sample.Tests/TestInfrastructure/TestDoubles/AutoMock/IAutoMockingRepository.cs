using System;
using Castle.Core;
using Castle.MicroKernel;
using Castle.Windsor;
using Rhino.Mocks;

namespace Sample.Tests.TestInfrastructure.TestDoubles.AutoMock
{
	public interface IAutoMockingRepository : IWindsorContainer
	{
		MockingStrategy GetStrategyFor(DependencyModel model);
		void AddStrategy(Type serviceType, MockingStrategy strategy);
		void OnMockCreated(Object mock, String dependencyName);
	    Boolean CanSatisfyDependencyKey(String dependencyKey);

	    /// <summary>
	    /// If false the container will not populate properties with mock.
	    /// </summary>
	    /// <value><c>true</c> if [resolve properties]; otherwise, <c>false</c>.</value>
	    Boolean ResolveProperties { get; set; }
	}
}