using LogVisualizer.Infrastructure;
using LogVisualizer.KissMvvm;

namespace LogVisualizer.ViewModels
{
    public class LogMessageViewModel : BaseViewModel
    {

        public LogMessage Log
        {
            get { return _Log; }
            set { this.Set(p => p.Log, value, ref _Log); }
        }

        private LogMessage _Log;

        public LogMessageViewModel() { }

        public LogMessageViewModel(LogMessage message) 
        {
            _Log = message;
        }

    }
}
