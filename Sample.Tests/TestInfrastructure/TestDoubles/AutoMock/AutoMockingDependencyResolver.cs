//using AutoMockingContainerSource;
using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.MicroKernel;
using Rhino.Mocks;
using System.Linq;
using Castle.MicroKernel.Context;

namespace Sample.Tests.TestInfrastructure.TestDoubles.AutoMock
{
	public class AutoMockingDependencyResolver : ISubDependencyResolver
	{
		private readonly IAutoMockingRepository _relatedRepository;


		public AutoMockingDependencyResolver(IAutoMockingRepository relatedRepository)
		{
			_relatedRepository = relatedRepository;
		}

		#region ISubDependencyResolver Members

		public bool CanResolve(
			 CreationContext context,
			 ISubDependencyResolver parentResolver,
			 ComponentModel model,
			 DependencyModel dependency)
		{
		    bool shouldResolveDependencyKey = 
		        _relatedRepository.CanSatisfyDependencyKey(dependency.DependencyKey);

		    Boolean resolveIfProperty = 
                !dependency.IsOptional || 
                _relatedRepository.ResolveProperties;

		    return shouldResolveDependencyKey && resolveIfProperty;
		}


	    public object Resolve(
			 CreationContext context,
			 ISubDependencyResolver parentResolver,
			 ComponentModel model,
			 DependencyModel dependency)
		{
			MockingStrategy strategy = _relatedRepository.GetStrategyFor(dependency);

			if (strategy.Instance != null)
				return strategy.Instance;
			if (strategy.Mock == MockingStrategyType.Mock)
			{
				//if a dependencywas already registered in the main controller, go and use it
				var registration = this._relatedRepository.Kernel.GetHandler(dependency.TargetType);
				object resolvedDependencyObject;
				if (registration == null)
				{
					resolvedDependencyObject = MockRepository.GenerateStub(dependency.TargetType);
				}
				else
				{
					resolvedDependencyObject = _relatedRepository.Resolve(dependency.TargetType);
				}
				_relatedRepository.OnMockCreated(resolvedDependencyObject, dependency.DependencyKey);
				return resolvedDependencyObject;
			}
			if (strategy.Mock == MockingStrategyType.Resolve)
				return _relatedRepository.Resolve(dependency.TargetType);

			return null;
		}

		#endregion

	}
}