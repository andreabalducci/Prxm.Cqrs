using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using LogVisualizer.KissMvvm;

namespace LogVisualizer.ViewModels
{
    public class SpecificLoggerViewModel : BaseViewModel
    {
        public SpecificLoggerViewModel(String name, ObservableCollection<LogMessageViewModel> logs)
        {
            LoggerName = name;
            Logs = new CollectionViewSource();
            Logs.Source = logs;
            Logs.Filter += LogsFilter;
            AllLogs = logs;
        }

        private void LogsFilter(object sender, FilterEventArgs e)
        {
            LogMessageViewModel vm = (LogMessageViewModel)e.Item;
            e.Accepted = LoggerName.Equals(vm.Log.Logger, StringComparison.OrdinalIgnoreCase);
        }

        public String LoggerName
        {
            get { return _LoggerName; }
            set { this.Set(p => p.LoggerName, value, ref _LoggerName); }
        }

        private String _LoggerName;

        public CollectionViewSource Logs { get; set; }

        public ObservableCollection<LogMessageViewModel> AllLogs {get; set;}

    }
}
