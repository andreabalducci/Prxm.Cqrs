using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.DebugUi.Infrastructure;
using Sample.DebugUi.KissMvvm;

namespace Sample.DebugUi.ViewModels
{
    public class LogMessageViewModel : BaseViewModel
    {

        public LogMessage Log
        {
            get { return _Log; }
            set { this.Set(p => p.Log, value, ref _Log); }
        }

        private LogMessage _Log;

        public LogMessageViewModel(LogMessage message) 
        {
            _Log = message;
        }

    }
}
