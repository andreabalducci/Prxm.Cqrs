using System;
using System.Linq;
using System.Collections.ObjectModel;
using LogVisualizer.KissMvvm;

namespace LogVisualizer.ViewModels
{
    /// <summary>
    /// Needed to aggregate more than one message in a single object, it is useful to group toghether
    /// logs like handlers and command and each one that should have an operation type, or each one that
    /// should be put in specific evidence, like NHibernate.SQL
    /// </summary>
    public class AggregateLogMessageViewModel : BaseViewModel
    {
        public AggregateLogMessageViewModel() 
        {
            Logs = new ObservableCollection<LogMessageViewModel>();
            Logs.CollectionChanged += LogsCollectionChanged;
        }

        void LogsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HigherLevelMessage = Logs.Select(lm => lm.Log.NumericLevel).Max();
        }

        /// <summary>
        /// Description that identify this specific AggregateLog.
        /// </summary>
        public String Identifier
        {
            get { return _Identifier; }
            set { this.Set(p => p.Identifier, value, ref _Identifier); }
        }

        private String _Identifier;

        /// <summary>
        /// Higher level of the message.
        /// </summary>
        public Int32 HigherLevelMessage
        {
            get { return _HigherLevelMessage; }
            set { this.Set(p => p.HigherLevelMessage, value, ref _HigherLevelMessage); }
        }

        private Int32 _HigherLevelMessage;

        public ObservableCollection<LogMessageViewModel> Logs { get; set; }


    }
}
