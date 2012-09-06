using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LogVisualizer.KissMvvm;

namespace LogVisualizer.ViewModels
{
    /// <summary>
    /// LoggerViewModel, it contains information about a single logger, it assumes that 
    /// loggers are separated by point value.
    /// </summary>
    public class LoggerViewModel : BaseViewModel
    {
        public LoggerViewModel(String name) 
        {
            LoggerName = name;
            Childs = new ObservableCollection<LoggerViewModel>();
        }

        public void ParseLogger(String logger) 
        {
            ParseLogger(logger.Split('.'), 0);
        }

        public void ParseLogger(String[] loggerParts, Int32 level) 
        {
            if (level == LoggerLevel) 
            {
                LoggerCount++;
                return;
            } 
            //do I have a child logger for this name?
            String name = loggerParts.Take(level + 1).Aggregate((s1, s2) => s1 + "." + s2);
            var cvm = Childs.SingleOrDefault(vm => vm.LoggerPrefix.Equals(name));
            if (cvm == null) 
            {
                cvm = new LoggerViewModel(name);
                Childs.Add(cvm);
            }
            cvm.ParseLogger(loggerParts, level + 1);
        }

        public Int32 LoggerLevel
        {
            get { return _LoggerLevel; }
            set { this.Set(p => p.LoggerLevel, value, ref _LoggerLevel); }
        }

        private Int32 _LoggerLevel;

        public Int32 LoggerCount
        {
            get { return _LoggerCount; }
            set { this.Set(p => p.LoggerCount, value, ref _LoggerCount); }
        }

        private Int32 _LoggerCount;

        public String LoggerName
        {
            get { return _LoggerName; }
            set { this.Set(p => p.LoggerName, value, ref _LoggerName); }
        }

        private String _LoggerName;

        /// <summary>
        /// This is the fullPrefix for the logger, it is useful to do easy filtering.
        /// </summary>
        public String LoggerPrefix
        {
            get { return _LoggerPrefix; }
            set { this.Set(p => p.LoggerPrefix, value, ref _LoggerPrefix); }
        }

        private String _LoggerPrefix;

        /// <summary>
        /// This is the collection of childs
        /// </summary>
        public ObservableCollection<LoggerViewModel> Childs { get; set; }
    }
}
