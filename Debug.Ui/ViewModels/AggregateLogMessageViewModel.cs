using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.DebugUi.KissMvvm;
using System.Collections.ObjectModel;
using Sample.DebugUi.Infrastructure;

namespace Sample.DebugUi.ViewModels
{
    /// <summary>
    /// Needed to aggregate more than one message in a single object
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
