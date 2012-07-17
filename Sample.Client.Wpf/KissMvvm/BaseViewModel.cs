using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;

namespace Sample.Client.Wpf.KissMvvm
{
    public abstract class BaseViewModel :  INotifyPropertyChanging
    {
       
        public virtual event PropertyChangingEventHandler PropertyChanging;

        private void OnPropertyChanging(String propertyName)
        {
            PropertyChangingEventHandler temp = PropertyChanging;
            if (temp != null)
            {

                temp(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String propertyName)
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
                SynchronizationContext.Current.Post(delegate { OnPropertyChanged(propertyName); }, null);
            else
                App.ExecuteInUiThread(() => OnPropertyChanged(propertyName));
        }
  


    }
}
