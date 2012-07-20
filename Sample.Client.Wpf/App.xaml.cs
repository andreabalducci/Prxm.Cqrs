using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using System.Reflection;
using Sample.Client.Wpf.KissMvvm;
using Sample.Client.Wpf.Views;

namespace Sample.Client.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Dispatcher _uiDispatcher;
        internal WindsorContainer Container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _uiDispatcher = this.Dispatcher;
            Container = new WindsorContainer();
            Container.Register(
                    Classes.FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<BaseViewModel>()
                    .LifestyleTransient());
            Container.Register(
               Classes.FromAssembly(Assembly.GetExecutingAssembly())
               .InNamespace("Sample.Client.Wpf.Views")
               .LifestyleTransient());

            var viewStart = Container.Resolve<MainWindow>();
            viewStart.Show();
        }

        public static void ExecuteInUiThread(Action action)
        {
            _uiDispatcher.Invoke((Delegate)action, new Object[] { });

        }
    }
}
