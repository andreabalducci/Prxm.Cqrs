using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;

namespace Sample.Tests.TestInfrastructure.TestDoubles.AutoMock
{
	public class StrategyKey
	{
		public Type TypeKey { get; set; }
		public String DependencyName { get; set; }

		public StrategyKey(Type typeKey, string dependencyName)
		{
			TypeKey = typeKey;
			DependencyName = dependencyName;
		}

		public Boolean IsValidFor(DependencyModel model)
		{
			if (model.DependencyKey == DependencyName ||
				model.TargetType == TypeKey)
				return true;

			return false;
		}
	}
}
