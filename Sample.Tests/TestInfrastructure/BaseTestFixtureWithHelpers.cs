using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Tests.TestInfrastructure
{
    public abstract class BaseTestFixtureWithHelper : BaseTestFixture
    {

        protected List<ITestHelper> _helpers = new List<ITestHelper>();

        protected IEnumerable<ITestHelper> Helpers { get { return _helpers.OrderByDescending(h => h.Priority); } }
        #region Rhino mocks helper

        private readonly Attribute[] customAttributes;


        #endregion

        protected BaseTestFixtureWithHelper()
        {
            Type type = this.GetType();
            customAttributes = Attribute.GetCustomAttributes(type);
            foreach (ITestHelperAttribute attribute in
                customAttributes.OfType<ITestHelperAttribute>())
            {
                _helpers.Add(attribute.Create());
            }
        }

        protected override void OnTestFixtureSetUp()
        {
            foreach (var helper in Helpers) helper.FixtureSetUp(this);
            base.OnTestFixtureSetUp();
        }

        protected override void OnSetUp()
        {
            foreach (var helper in Helpers) helper.SetUp(this);
            base.OnSetUp();
        }

        protected override void OnTearDown()
        {
            foreach (var helper in Helpers) helper.TearDown(this);
            base.OnTearDown();
        }

        protected override void OnTestFixtureTearDown()
        {
            foreach (var helper in Helpers) helper.FixtureTearDown(this);
            base.OnTestFixtureTearDown();
        }
    }

    /// <summary>
    /// A test helper is an object that can be used to interact
    /// with the test
    /// </summary>
    public interface ITestHelper
    {
        void FixtureSetUp(BaseTestFixture fixture);
        void SetUp(BaseTestFixture fixture);
        void TearDown(BaseTestFixture fixture);
        void FixtureTearDown(BaseTestFixture fixture);

        /// <summary>
        /// indica la priorità degli helper, serve perchè alcuni debbono forzatamente
        /// girare prima degli altri, Es. quello che ricrea il db prima di qualsiasi altro helper che legge il db. <br />
        /// Il valore più è alto più è importante, di base i plugin dovrebbero tornare 1 che significa che possono girare
        /// dopo tutti gli altri.
        /// </summary>
        Int32 Priority { get; }

    }

    public interface ITestHelperAttribute
    {
        ITestHelper Create();
    }
}
