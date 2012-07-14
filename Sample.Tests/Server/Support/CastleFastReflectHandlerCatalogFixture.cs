using System;
using NUnit.Framework;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Server.Commanding;
using Proximo.Cqrs.Server.Impl;
using Sample.Tests.TestInfrastructure;
using Sample.Tests.TestInfrastructure.TestDoubles.AutoMock;
using Castle.MicroKernel;
using Rhino.Mocks;
using SharpTestsEx;
using Proximo.Cqrs.Server.Eventing;

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

        [Test]
        public void Verify_that_assembly_are_scanned_correctly_for_event_handler()
        {
            //Verify that I'm able to execute Testcommand
            var listOfHandlers = sut.GetAllHandlerFor(typeof(MyDomainEvent));
            listOfHandlers.Should().Have.Count.GreaterThan(0);
        }

        [Test]
        public void Verify_that_handler_can_be_invoked_correctly()
        {
            //Verify that I'm able to execute Testcommand
            var listOfHandlers = sut.GetAllHandlerFor(typeof(MyDomainEvent));
            MyDomainEvent evt = new MyDomainEvent();
            foreach (var handler in listOfHandlers)
            {
                handler(evt);
            }
            evt.CallCount.Should().Be.EqualTo(1);
        }

        [Test]
        public void Verify_we_can_create_a_catch_event_from_base_type()
        {
            var listOfHandlers = sut.GetAllHandlerFor(typeof(MyBaseDomainEvent));
            MyBaseDomainEvent evtbase = new MyBaseDomainEvent();
            foreach (var handler in listOfHandlers)
            {
                handler(evtbase);
            }
            evtbase.CallCount.Should().Be.EqualTo(1);

            MyDerivedDomainEvent evtderived = new MyDerivedDomainEvent();
            foreach (var handler in listOfHandlers)
            {
                handler(evtderived);
            }
            evtderived.CallCount.Should().Be.EqualTo(1);
        }

        [Test]
        public void Verify_handler_of_derived_class()
        {
            var listOfHandlers = sut.GetAllHandlerFor(typeof(MyDerivedDomainEvent));
            MyDerivedDomainEvent evtderived = new MyDerivedDomainEvent();
            foreach (var handler in listOfHandlers)
            {
                handler(evtderived);
            }
            evtderived.CallCountSpecific.Should().Be.EqualTo(1);
        }
    }



    #region Command helper classes

    public class TestCommand : ICommand
    {
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

    #endregion

    #region Domain Handler helper classes

    public class MyDomainEvent : DomainEvent { public Int32 CallCount { get; set; } }

    public class MyBaseDomainEvent : DomainEvent { public Int32 CallCount { get; set; } }

    public class MyDerivedDomainEvent : MyBaseDomainEvent, IDomainEvent { public Int32 CallCountSpecific { get; set; } }

    public class EventHandler1 : IDomainEventHandler
    {

        public void BariBari(MyDomainEvent evt)
        {

            evt.CallCount++;
        }

        public void Catch_derived_event(MyDerivedDomainEvent evt)
        {
            evt.CallCountSpecific++;
        }
    }

    public class EventHandlerBase : IDomainEventHandler
    {
        public void handle_can_call_me_whathever_u_wanna(MyBaseDomainEvent baseEvent)
        {
            baseEvent.CallCount++;
        }


    }

    #endregion


}
