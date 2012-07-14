using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Castle.Windsor;
using Rhino.Mocks;

namespace Sample.Tests.TestInfrastructure.TestDoubles.AutoMock
{

    public class AutoMockingContainer : WindsorContainer, IAutoMockingRepository
    {
        public AutoMockingContainer()
        {
            _autoMockingFacility = new AutoMockingFacility(this);
            this.AddFacility("Automocking", _autoMockingFacility);
            DependencyToIgnore = new List<string>();
            ResolveProperties = true;
        }

        private Dictionary<Type, List<Object>> _mocks = new Dictionary<Type, List<Object>>();

        public event EventHandler<MockCreatedEventArgs> MockCreated;

        public void OnMockCreated(Object mock, String dependencyName)
        {
            if (!_mocks.ContainsKey(mock.GetType()))
            {
                _mocks.Add(mock.GetType(), new List<object>());
            }
            _mocks[mock.GetType()].Add(mock);
            EventHandler<MockCreatedEventArgs> temp = MockCreated;
            if (temp != null)
            {
                temp(this, new MockCreatedEventArgs(mock, dependencyName));
            }
        }

        private Dictionary<StrategyKey, MockingStrategy> _strategies
             = new Dictionary<StrategyKey, MockingStrategy>();

        private AutoMockingFacility _autoMockingFacility;

        void IAutoMockingRepository.AddStrategy(Type serviceType, MockingStrategy strategy)
        {
            _strategies[new StrategyKey(serviceType, String.Empty)] = strategy;
        }

        public void ClearAllStrategies()
        {
            _strategies.Clear();
        }

        public void MarkNonMocked<T>()
        {
            MarkNonMocked(typeof(T));
        }

        public void SetStrategyForDependencyName(string p, MockingStrategy strategy)
        {
            _strategies.Add(new StrategyKey(null, p), strategy);
        }

        public void MarkNonMocked(Type t)
        {
            var strategies = _strategies
              .Where(kvp => kvp.Key.TypeKey == t)
              .Select(kvp => kvp.Value);
            foreach (var strategy in strategies)
            {
                strategy.Mock = MockingStrategyType.Resolve;
            }
        }

        public void RegisterInstance<T>(T instance)
        {
            RegisterInstance(typeof(T), instance);
        }


        public void RegisterInstance(Type serviceType, Object instance)
        {
            _strategies[new StrategyKey(serviceType, String.Empty)] = new MockingStrategy() { Instance = instance };
        }

        #region IAutoMockingRepository Members

        public MockingStrategy GetStrategyFor(Castle.Core.DependencyModel model)
        {
            MockingStrategy strategy = _strategies
                .Where(kvp => kvp.Key.IsValidFor(model))
                .Select(kvp => kvp.Value)
                .FirstOrDefault();
            return strategy ?? MockingStrategy.Default;
        }

        /// <summary>
        /// These are the dependencies to ignore, sometimes for some object we need to automock
        /// not everything, expecially public properties, so we can setup an ignorelist.
        /// </summary>
        /// <value>The dependency to ignore.</value>
        public List<String> DependencyToIgnore { get; set; }

        /// <summary>
        /// If false the container will not populate properties with mock.
        /// </summary>
        /// <value><c>true</c> if [resolve properties]; otherwise, <c>false</c>.</value>
        public Boolean ResolveProperties { get; set; }

        /// <summary>
        /// Determines whether this instance [can satisfy dependency key] the specified dependency key.
        /// </summary>
        /// <param name="dependencyKey">The dependency key.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can satisfy dependency key] the specified dependency key; otherwise, <c>false</c>.
        /// </returns>
        public Boolean CanSatisfyDependencyKey(String dependencyKey)
        {
            return !DependencyToIgnore.Contains(dependencyKey);
        }

        #endregion

        #region Get Created Mock

        public T GetFirstCreatedMock<T>()
        {
            if (_mocks.ContainsKey(typeof(T)))
            {
                return (T)_mocks[typeof(T)].Last();
            }
            foreach (KeyValuePair<Type, List<object>> keyValuePair in _mocks)
            {
                if (typeof(T).IsAssignableFrom(keyValuePair.Key))
                    return (T)keyValuePair.Value.First();
            }
            return default(T);
        }

        public List<T> GetMock<T>()
        {
            return _mocks[typeof(T)].Cast<T>().ToList();
        }
        #endregion

        #region Resolve mock

        public override T Resolve<T>(string key)
        {
            if (IsTypeRegistered(typeof(T)))
            {
                return base.Resolve<T>(key);

            }
            return (T)MockRepository.GenerateStub(typeof(T));
        }

        public new T Resolve<T>()
        {
            if (IsTypeRegistered(typeof(T)))
            {
                return base.Resolve<T>();

            }
            return (T)MockRepository.GenerateStub(typeof(T));
        }

        private Boolean IsTypeRegistered(Type service)
        {
            return this.Kernel.GetHandler(service) != null;
        }

        #endregion
    }

}