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

        public RawLoggerViewModel(ILogInterceptor logIntercetptor)
        {

            _interceptor = logIntercetptor;
            _Logs = new ObservableCollection<LogMessageViewModel>();
            Commands = new ObservableCollection<AggregateLogMessageViewModel>();
            Handlers = new ObservableCollection<AggregateLogMessageViewModel>();
            AggregatedLogs = new ObservableCollection<OpTypeLoggerViewModel>();
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
            App.ExecuteInUiThread(() =>
            {
                _Logs.Add(new LogMessageViewModel(e.Message));
                if (!String.IsNullOrEmpty(e.Message.OpType))
                {
                  
                    if ("command".Equals(e.Message.OpType, StringComparison.OrdinalIgnoreCase))
                    {
                        //I already have a logger for this command?
                        var alvm = Commands.SingleOrDefault(c => c.Identifier == e.Message.OpTypeId);
                        if (alvm == null)
                        {
                            alvm = new AggregateLogMessageViewModel() { Identifier = e.Message.OpTypeId };
                            Commands.Add(alvm);
                        }
                        alvm.Logs.Add(new LogMessageViewModel( e.Message));
                    }
                    else if ("event".Equals(e.Message.OpType, StringComparison.OrdinalIgnoreCase))
                    {
                        //I already have a logger for this command?
                        var alvm = Handlers.SingleOrDefault(c => c.Identifier == e.Message.OpTypeId);
                        if (alvm == null)
                        {
                            alvm = new AggregateLogMessageViewModel() { Identifier = e.Message.OpTypeId };
                            Handlers.Add(alvm);
                        }
                        alvm.Logs.Add(new LogMessageViewModel(e.Message));
                    }
                    else { 
                        //Generic optype
                        var alogs = AggregatedLogs.SingleOrDefault(al => al.OperationType.Equals(e.Message.OpType));
                        if (alogs == null) {
                            alogs = new OpTypeLoggerViewModel(e.Message.OpType);
                            AggregatedLogs.Add(alogs);
                        }
                        //now I have the container for the aggregate
                        var aggl = alogs.AggregatedLogs.SingleOrDefault(c => c.Identifier == e.Message.OpTypeId);
                        if (aggl == null)
                        {
                            aggl = new AggregateLogMessageViewModel() { Identifier = e.Message.OpTypeId };
                            alogs.AggregatedLogs.Add(aggl);
                        }
                        aggl.Logs.Add(new LogMessageViewModel(e.Message));
                    }
                }
            });
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
            LogMessageViewModel vm = (LogMessageViewModel)e.Item;

            //first of all filter for exact level.
            if (!String.IsNullOrEmpty(LevelFilter) && !vm.Log.Level.Equals(LevelFilter, StringComparison.OrdinalIgnoreCase))
            {
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

        #region Grouping

        public ObservableCollection<AggregateLogMessageViewModel> Commands {get; set;}

        /// <summary>
        /// this is the selected command
        /// </summary>
        public AggregateLogMessageViewModel SelectedCommand
        {
            get { return _SelectedCommand; }
            set { this.Set(p => p.SelectedCommand, value, ref _SelectedCommand); }
        }

        private AggregateLogMessageViewModel _SelectedCommand;

        public ObservableCollection<AggregateLogMessageViewModel> Handlers { get; set; }

        /// <summary>
        /// this is the selected command
        /// </summary>
        public AggregateLogMessageViewModel SelectedHandler
        {
            get { return _SelectedHandler; }
            set { this.Set(p => p.SelectedHandler, value, ref _SelectedHandler); }
        }

        private AggregateLogMessageViewModel _SelectedHandler;

        /// <summary>
        /// This is for generic aggregate logs that does not fell into Commands or handlers, they
        /// are partitioned by the op_type
        /// </summary>
        public ObservableCollection<OpTypeLoggerViewModel> AggregatedLogs { get; set; }

        #endregion

        #region Commands

        public void ExecuteClearAll(Object param) 
        {
            _Logs.Clear();
            SelectedLog = null;
            SelectedCommand = null;
            SelectedHandler = null;
            Commands.Clear();
            Handlers.Clear();
            AggregatedLogs.Clear();
        }

        public void ExecuteClearOnlyLogs(Object param)
        {
            _Logs.Clear();
            SelectedLog = null;
        }

        public void ExecuteClearCommandsLogs(Object param)
        {
            SelectedCommand = null;
            Commands.Clear();
        }

        public void ExecuteClearHandlersLogs(Object param)
        {
            SelectedHandler = null;
            Handlers.Clear();
        }

        public void ExecuteClearAggregatedLogs(Object param)
        {
            AggregatedLogs.Clear();
        }

        #endregion
    }
}
