using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;

namespace Sample.Client.Wpf.KissMvvm
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        protected void OnPropertyChanged<T>(Expression<Func<T>> accessor)
        {
            RaisePropertyChanged(accessor.GetMemberName());
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            if (PropertyChanged == null) return;
            if (SynchronizationContext.Current != null)
                SynchronizationContext.Current.Post(delegate
                {
                    OnPropertyChanged(propertyName);
                }, null);
            else
            {
                App.ExecuteInUiThread(() => OnPropertyChanged(propertyName));
            }
        }

        #region AsyncExecution

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public String WaitMessage
        {
            get { return _waitMessage; }
            set { this.Set(p => p.WaitMessage, value, ref _waitMessage); }
        }
        private String _waitMessage;

        public Boolean IsBusy
        {
            get { return _IsBusy; }
            set { this.Set(p => p.IsBusy, value, ref _IsBusy); }
        }

        private Boolean _IsBusy;

        /// <summary>
        /// Esegue una azione in modalità asincrona o sincrona a seconda 
        /// della modalitaà di funzionamento impostata, durante l'esecuzione
        /// viene messo o meno lo stato a busy.
        /// </summary>
        /// <param name="action"></param>
        protected void Execute(Action action)
        {
            Execute(action, false);
        }

        /// <summary>
        /// Esegue una azione in modalità asincrona o sincrona a seconda 
        /// della modalitaà di funzionamento impostata, durante l'esecuzione
        /// viene messo o meno lo stato a busy.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="setViewBusy"></param>
        internal protected void Execute(Action action, bool setViewBusy)
        {
            Execute(action, setViewBusy, "Executing Command ...");
        }

        public Boolean SyncExecution { get; set; }

        /// <summary>
        /// Esegue una azione in modalità asincrona o sincrona a seconda 
        /// della modalitaà di funzionamento impostata, durante l'esecuzione
        /// viene messo o meno lo stato a busy.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="setViewBusy"></param>
        /// <param name="waitMessage"></param>
        internal protected void Execute(Action action, bool setViewBusy, string waitMessage)
        {
            InnerExecute(action, setViewBusy, waitMessage);
        }

        protected void SetBusy(String message)
        {
            IsBusy = true;
            WaitMessage = message;
        }

        private void InnerExecute(Action action, bool setViewBusy, string waitMessage)
        {
            if (SyncExecution)
            {
                action();
            }
            else
            {
                if (!setViewBusy)
                {
                    ThreadPool.QueueUserWorkItem(o => action());
                }
                else
                {
                    IsBusy = true;
                    WaitMessage = waitMessage;
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        try
                        {
                            action();
                        }
                        finally
                        {
                            IsBusy = false;
                            WaitMessage = String.Empty;
                        }
                    });
                }

            }
        }

        #endregion


    }
}
