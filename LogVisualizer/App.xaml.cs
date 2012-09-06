using System;
using System.Windows;
using System.Windows.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System.Reflection;
using Castle.Facilities.Startable;
using LogVisualizer.Infrastructure;
using LogVisualizer.ViewModels;
using LogVisualizer.Views;

namespace LogVisualizer
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
               AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
               .Where(t => t.Namespace == "LogVisualizer.Views" || t.Namespace == "LogVisualizer.ViewModels")
               .Configure(c => c.LifeStyle.Transient));
            Container.Register(
                Component.For<ILogInterceptor>().ImplementedBy<UdpInterceptor>());
            var viewStart = Container.Resolve<RawLoggerView>();
            viewStart.Show();

        }

        public static void ExecuteInUiThread(Action action)
        {
            _uiDispatcher.Invoke((Delegate)action, new Object[] { });

        }

     
    }
}
