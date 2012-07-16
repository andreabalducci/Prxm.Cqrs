using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Sample.Client.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Dispatcher _uiDispatcher;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _uiDispatcher = this.Dispatcher;
        }

        public static void ExecuteInUiThread(Action action)
        {
            _uiDispatcher.Invoke((Delegate)action, new Object[] { });

        }
    }
}
