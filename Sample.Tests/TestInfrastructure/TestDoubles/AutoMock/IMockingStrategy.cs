using System;
using System.Reflection;
using Castle.MicroKernel;
using Rhino.Mocks;

namespace Sample.Tests.TestInfrastructure.TestDoubles.AutoMock
{
	public enum MockingStrategyType
	{
		Mock,
		Resolve,
		NoAction
	}
	public class MockingStrategy
	{
		public readonly static MockingStrategy Default = new MockingStrategy() { Mock = MockingStrategyType.Mock };
		public readonly static MockingStrategy NoAction = new MockingStrategy() { Mock = MockingStrategyType.NoAction };
		public readonly static MockingStrategy Resolve = new MockingStrategy() { Mock = MockingStrategyType.Resolve };

		public MockingStrategy(object instance, MockingStrategyType mock)
		{
			Instance = instance;
			Mock = mock;
		}

		public MockingStrategy()
		{
			Mock = MockingStrategyType.Mock;
		}

		public Object Instance { get; set; }
		public MockingStrategyType Mock { get; set; }
	}
}