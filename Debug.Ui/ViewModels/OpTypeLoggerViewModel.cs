using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.DebugUi.KissMvvm;
using System.Collections.ObjectModel;

namespace Sample.DebugUi.ViewModels
{
    public class OpTypeLoggerViewModel : BaseViewModel
    {
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
