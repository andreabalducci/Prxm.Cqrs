using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Server.Commanding;
using Sample.Tests.TestInfrastructure;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Proximo.Cqrs.Core.Support;
using Sample.Tests.InProcessBusTests;
using Sample.Server.Support;
using Sample.Tests.TestInfrastructure.TestDoubles.AutoMock;
using Castle.MicroKernel;
using Rhino.Mocks;
using SharpTestsEx;

namespace Sample.Tests.Server.Support
{
    [TestFixture]
    [UseAutoMockingContainer(new Type[] { typeof(CastleFastReflectHandlerCatalog) })]
    public class CastleFastReflectHandlerCatalogFixture : BaseTestFixtureWithHelper
    {
        private CastleFastReflectHandlerCatalog sut;

        protected override void OnSetUp()
        {
            base.OnSetUp();

            sut = this.ResolveWithAutomock<CastleFastReflectHandlerCatalog>();
            //each object it want to resolve, I create with activator createinstance.
            this.GetMock<IKernel>()
                .Expect(k => k.Resolve(null))
                .IgnoreArguments()
                .Repeat.Any()
                .WhenCalled(action => action.ReturnValue = Activator.CreateInstance(action.Arguments[0] as Type));
        }
       

        [Test]
        public void Verify_that_assembly_are_scanned_correctly()
        {
            //Verify that I'm able to execute Testcommand
            sut.GetExecutorFor(typeof(TestCommand)).Should().Not.Be.Null();
        }

        [Test]
        public void Verify_that_executors_really_call_the_method()
        {
            //Verify that I'm able to execute Testcommand
            var cmd = new TestCommand();
            sut.GetExecutorFor(typeof(TestCommand))(cmd);
            cmd.CallCount.Should().Be.EqualTo(1);
        }

        [Test]
        public void Verify_multiple_executor_classes_are_scanned_correctly()
        {
            //TestCommand1 is handled by an executor with two methods
             var cmd1 = new TestCommand1();
            sut.GetExecutorFor(typeof(TestCommand1))(cmd1);
            cmd1.CallCount.Should().Be.EqualTo(1);

            var cmd2 = new TestCommand2();
            sut.GetExecutorFor(typeof(TestCommand2))(cmd2);
            cmd2.CallCount.Should().Be.EqualTo(1);
        }
    }

    public class TestCommand : ICommand { 
        public Guid Id { get; set; }
        public Int32 CallCount { get; set; }
    }

    public class TestCommand1 : ICommand
    {
        public Guid Id { get; set; }
        public Int32 CallCount { get; set; }
    }

    public class TestCommand2 : ICommand
    {
        public Guid Id { get; set; }
        public Int32 CallCount { get; set; }
    }

    public class TestCommandHandler : ICommandHandler
    {
        public void DoingTheTest(TestCommand testCommand)
        {
            testCommand.CallCount++;
        }
    }


    public class MultipleCommandHandler : ICommandHandler
    {
        public void DoingTheTest(TestCommand1 testCommand)
        {
            testCommand.CallCount++;
        }

        public void ExecutingAnotherCommand(TestCommand2 testCommand)
        {
            testCommand.CallCount++;
        }
    }
}
