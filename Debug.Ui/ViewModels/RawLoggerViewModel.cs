using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.DebugUi.KissMvvm;
using Sample.DebugUi.Infrastructure;
using System.Collections.ObjectModel;

namespace Sample.DebugUi.ViewModels
{
    public class RawLoggerViewModel : BaseViewModel
    {
        ILogInterceptor _interceptor;

        public RawLoggerViewModel(ILogInterceptor logIntercetptor) {

            _interceptor = logIntercetptor;
            Logs = new ObservableCollection<LogMessageViewModel>();
            _interceptor.LogIntercepted += LogIntercepted;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogIntercepted(object sender, LogInterceptedEventArgs e)
        {
            App.ExecuteInUiThread(() => Logs.Add(new LogMessageViewModel(e.Message)));
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<LogMessageViewModel> Logs { get; set; }
    }
}
