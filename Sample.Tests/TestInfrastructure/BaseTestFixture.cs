using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.Windsor;

namespace Sample.Tests.TestInfrastructure
{
    public abstract class BaseTestFixture
    {

        #region Initialization and private

        private List<IDisposable> singleTestDisposableList;
        private List<Action> singleTestTearDownActions;
        private List<IDisposable> fixtureDisposableList;
        private List<Action> fixtureTearDownActions;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            singleTestDisposableList = new List<IDisposable>();
            singleTestTearDownActions = new List<Action>();
            fixtureDisposableList = new List<IDisposable>();
            fixtureTearDownActions = new List<Action>();
            try
            {
                OnTestFixtureSetUp();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error during fixture setup {0}", ex);
                throw;
            }

        }

        protected virtual void OnTestFixtureSetUp()
        {
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Boolean ErrorOnDispose = false;
            fixtureDisposableList.ForEach(d =>
            {
                try
                {
                    d.Dispose();
                }
                catch (Exception)
                {
                    ErrorOnDispose = true;
                }
            });
            Boolean ErrorOnTearDownAction = false;
            fixtureTearDownActions.ForEach(a =>
            {
                try
                {
                    a();
                }
                catch (Exception)
                {
                    ErrorOnTearDownAction = true;
                }
            });
            Assert.That(ErrorOnDispose == false, "Some disposable object generates errors during Fixture Tear Down");
            Assert.That(ErrorOnTearDownAction == false, "Some tear down action generates errors during Fixture Tear Down");
            OnTestFixtureTearDown();
        }

        protected virtual void OnTestFixtureTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {

            singleTestDisposableList.Clear();
            singleTestTearDownActions.Clear();
            OnSetUp();
        }

        protected virtual void OnSetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            Boolean ErrorOnDispose = false;
            singleTestDisposableList.ForEach(d =>
            {
                try
                {
                    d.Dispose();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    ErrorOnDispose = true;
                }
            });
            Boolean ErrorOnTearDownAction = false;
            singleTestTearDownActions.ForEach(a =>
            {
                try
                {
                    a();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    ErrorOnTearDownAction = true;
                }
            });
            Assert.That(ErrorOnDispose == false, "Some disposable object generates errors during Test Tear Down");
            Assert.That(ErrorOnTearDownAction == false, "Some tear down action generates errors during Test Tear Down");
            OnTearDown();
        }

        protected virtual void OnTearDown()
        {
            TestContext.Clear();
        }

        #endregion

        #region Cleanup management

        public void DisposeAtTheEndOfTest(IDisposable disposableObject)
        {
            singleTestDisposableList.Add(disposableObject);
        }

        public void DisposeAtTheEndOfFixture(IDisposable disposableObject)
        {
            fixtureDisposableList.Add(disposableObject);
        }

        public void ExecuteAtTheEndOfTest(Action action)
        {
            singleTestTearDownActions.Add(action);
        }

        public void ExecuteAtTheEndOfFixture(Action action)
        {
            fixtureTearDownActions.Add(action);
        }

        #endregion

        #region Context

        private Dictionary<String, object> TestContext = new Dictionary<string, object>();
        public void SetIntoTestContext(String key, Object value)
        {
            if (!TestContext.ContainsKey(key))
                TestContext.Add(key, value);
            else
                TestContext[key] = value;
        }

        public T GetFromTestContext<T>(String key)
        {
            return (T)TestContext[key];
        }

        #endregion
    }
}
