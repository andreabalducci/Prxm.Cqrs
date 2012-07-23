using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Sample.DebugUi.Infrastructure;
using System.Windows.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Sample.DebugUi.KissMvvm;
using System.Reflection;
using Sample.DebugUi.Views;
using Castle.Facilities.Startable;
using Sample.DebugUi.ViewModels;
namespace Sample.DebugUi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Dispatcher _uiDispatcher;
        internal WindsorContainer Container;
        internal UdpInterceptor _interceptor;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _uiDispatcher = this.Dispatcher;
            Container = new WindsorContainer();

            Container.AddFacility<StartableFacility>();

            Container.Register(
                    Component.For<RawLoggerViewModel>().ImplementedBy<RawLoggerViewModel>());

            Container.Register(
               Classes.FromAssembly(Assembly.GetExecutingAssembly())
               .InNamespace("Sample.DebugUi.Views")
               .LifestyleTransient());
            Container.Register(
                Component.For<ILogInterceptor>().ImplementedBy<UdpInterceptor>().LifestyleSingleton());
            var viewStart = Container.Resolve<RawLoggerView>();
            viewStart.Show();

        }

        public static void ExecuteInUiThread(Action action)
        {
            _uiDispatcher.Invoke((Delegate)action, new Object[] { });

        }

     
    }
}
