using System;
using System.Collections.ObjectModel;
using LogVisualizer.KissMvvm;

namespace LogVisualizer.ViewModels
{
    /// <summary>
    /// Group toghether a series of logger that is raised by a specific OpType, each opType has a list of 
    /// Aggregate logs.
    /// </summary>
    public class OpTypeLoggerViewModel : BaseViewModel
    {
        public OpTypeLoggerViewModel() { }

        public OpTypeLoggerViewModel(String description) 
        {
            AggregatedLogs = new ObservableCollection<AggregateLogMessageViewModel>();
            OperationType = description;
        }

        public String OperationType
        {
            get { return _Description; }
            set { this.Set(p => p.OperationType, value, ref _Description); }
        }

        private String _Description;

        public AggregateLogMessageViewModel SelectedAggregateLog
        {
            get { return _SelectedAggregateLog; }
            set { this.Set(p => p.SelectedAggregateLog, value, ref _SelectedAggregateLog); }
        }

        private AggregateLogMessageViewModel _SelectedAggregateLog;

        public ObservableCollection<AggregateLogMessageViewModel> AggregatedLogs { get; set; }

    }
}
