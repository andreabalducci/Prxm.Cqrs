using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace LogVisualizer.KissMvvm
{
    public static class PropertyChangedObserver
    {
        public static PropertyChangedMonitor<T> Monitor<T>(T source)
            where T : class, INotifyPropertyChanged
        {
            return new PropertyChangedMonitor<T>(source);
        }

        /// <summary>
        /// Imposta la callback per un notifypropertychanged, per far si che sia
        /// possibile gestire il tutto in maniera comoda, la callback riceve comunque
        /// come parametro il nome della proprietà che è camboiata in questo modo l'utilizzatore
        /// può usare la stessa funzione per più callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static PropertyChangedMonitor<T> HandleChangesOf<T>(
            this T source,
            Expression<Func<T, Object>> property,
            Action<String> callback) where T : class, INotifyPropertyChanged
        {
            return PropertyChangedObserver.Monitor(source)
                .HandleChangesOf(property, callback);
        }
    }

    public class PropertyChangedMonitor<T> :
        INotifyPropertyChanged
        where T : class, INotifyPropertyChanged
    {

        protected WeakReference<T> WeakSource { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, args);
            }
        }

        IDictionary<String, Action<String>> propertiesToWatch = new Dictionary<String, Action<String>>();

        /// <summary>
        /// Questa è la funzione ch emonitora una proprietà di un oggetto
        /// </summary>
        /// <param name="source"></param>
        public PropertyChangedMonitor(T source)
        {
            WeakSource = new WeakReference<T>(source);
            source.PropertyChanged += TargetPropertyChanged;
        }

        void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (propertiesToWatch.ContainsKey(e.PropertyName))
            {
                propertiesToWatch[e.PropertyName](e.PropertyName);
            }
        }

        public PropertyChangedMonitor<T> HandleChangesOf<TProperty>(Expression<Func<T, TProperty>> property, Action<String> callback)
        {
            var propertyName = property.GetMemberName();
            if (propertiesToWatch.ContainsKey(propertyName))
            {
                propertiesToWatch[propertyName] = callback;
            }
            else
            {
                propertiesToWatch.Add(propertyName, callback);
            }
            return this;
        }

        /// <summary>
        /// Ferma il monitoraggio della proprietà-.
        /// </summary>
        public void OnStopMonitoring()
        {
            if (this.WeakSource != null && this.WeakSource.IsAlive)
            {
                this.WeakSource.Target.PropertyChanged -= TargetPropertyChanged;
            }
        }
    }
}
