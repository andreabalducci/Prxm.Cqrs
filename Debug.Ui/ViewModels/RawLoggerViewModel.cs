using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.DebugUi.KissMvvm;
using Sample.DebugUi.Infrastructure;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections;

namespace Sample.DebugUi.ViewModels
{
    public class RawLoggerViewModel : BaseViewModel
    {
        ILogInterceptor _interceptor;

        public RawLoggerViewModel(ILogInterceptor logIntercetptor) {

            _interceptor = logIntercetptor;
            _Logs = new ObservableCollection<LogMessageViewModel>();
            CvsLogs = new CollectionViewSource();
            CvsLogs.Source = _Logs;
            CvsLogs.Filter += CvsLogsFilter;
            Logs = CvsLogs.View;
            _interceptor.LogIntercepted += LogIntercepted;

            PropertyChangedObserver.Monitor(this)
                .HandleChangesOf(vm => vm.LevelFilter, EvaluateFilter)
                .HandleChangesOf(vm => vm.MainFilter, EvaluateFilter);
        }

        public void EvaluateFilter(String pname) 
        {
            CvsLogs.View.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogIntercepted(object sender, LogInterceptedEventArgs e)
        {
            App.ExecuteInUiThread(() => _Logs.Add(new LogMessageViewModel(e.Message)));
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<LogMessageViewModel> _Logs { get; set; }

        public CollectionViewSource CvsLogs { get; set; }

        public Object Logs
        {
            get { return _logs; }
            set { this.Set(p => p.Logs, value, ref _logs); }
        }

        private Object _logs;


        /// <summary>
        /// Represents the selected log.
        /// </summary>
        public LogMessageViewModel SelectedLog
        {
            get { return _SelectedLog; }
            set { this.Set(p => p.SelectedLog, value, ref _SelectedLog); }
        }

        private LogMessageViewModel _SelectedLog;

        #region Filtering

        public String LevelFilter
        {
            get { return _LevelFilter; }
            set { this.Set(p => p.LevelFilter, value, ref _LevelFilter); }
        }

        private String _LevelFilter;

        public String MainFilter
        {
            get { return _MainFilter; }
            set { this.Set(p => p.MainFilter, value, ref _MainFilter); }
        }

        private String _MainFilter;

        void CvsLogsFilter(object sender, FilterEventArgs e)
        {
             LogMessageViewModel vm = (LogMessageViewModel) e.Item;

             //first of all filter for exact level.
             if (!String.IsNullOrEmpty(LevelFilter) && !vm.Log.Level.Equals(LevelFilter, StringComparison.OrdinalIgnoreCase)) {
                 e.Accepted = false;
                 return;
             }
             //now filter for main filter
             if (!String.IsNullOrEmpty(MainFilter)) 
             {
                 e.Accepted = vm.Log.Message.Contains(MainFilter, StringComparison.OrdinalIgnoreCase) || 
                     vm.Log.Logger.Contains(MainFilter, StringComparison.OrdinalIgnoreCase);
             }
        }

	
        #endregion
    }
}
