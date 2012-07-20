using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Sample.Client.Wpf.KissMvvm
{
    /// <summary>
    /// Alle volte accade che nei viewmodel ci siano proprietà readonly
    /// che sono determinate da altre, con questa classe io posso linkare
    /// la notifica di <see cref="INotifyPropertyChanged"/> di una proprietà
    /// ad alcune altre.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="O"></typeparam>
    internal class PropertyLinkMonitor<T, O>
        where T :  BaseViewModel
        where O : class, INotifyPropertyChanged
    {
        /// <summary>
        /// L'originatore degli eventi, può anche esser lo stesso oggetti, ma non
        /// è detto per essere più flessibili.
        /// </summary>
        private WeakReference<O> Originator;

        /// <summary>
        /// La sorgente dell'evento.
        /// </summary>
        private WeakReference<T> Source;

        /// <summary>
        /// In sostanza monitora alcune notify di originator e le rilancia da source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="originator">The originator.</param>
        public PropertyLinkMonitor(T source, O originator)
        {
            this.Source = new WeakReference<T>(source);
            this.Originator = new WeakReference<O>(originator);
            this.Originator.Target.PropertyChanged += HandlePropertyChanges;
        }

        public PropertyLinkMonitor<T, O> Link<TProperty1, TProperty2>(
            Expression<Func<O, TProperty1>> propertySource,
            Expression<Func<T, TProperty2>> propertyDest)
        {
            var propertyName = propertySource.GetMemberName();
            if (!links.ContainsKey(propertyName))
            {
                links.Add(propertyName, new List<string>());
            }
            links[propertyName].Add(propertyDest.GetMemberName());
            return this;
        }

        public PropertyLinkMonitor<T, O> Link<TProperty1>(
            Expression<Func<O, TProperty1>> propertySource,
            String propertyDest)
        {
            var propertyName = propertySource.GetMemberName();
            if (!links.ContainsKey(propertyName))
            {
                links.Add(propertyName, new List<string>());
            }
            links[propertyName].Add(propertyDest);
            return this;
        }

        private Dictionary<String, List<String>> links = new Dictionary<string, List<string>>();

        /// <summary>
        /// Debbo notificare al cambiamento di una proprietà anche il cambiamento di altre
        /// proprietà
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void HandlePropertyChanges(Object sender, PropertyChangedEventArgs e)
        {

            List<String> related;
            if (links.TryGetValue(e.PropertyName, out related))
            {
                if (related != null)
                {
                    if (Source.IsAlive)
                    {
                        T pinSourced = Source.Target;
                        foreach (var relatedProperty in related)
                        {
                            pinSourced.RaisePropertyChanged(relatedProperty);
                        }
                    }
                }
            }
        }

    }

    internal class PropertyLink
    {
        /// <summary>
        /// Lega una proprietà ad un altra, è il punto di partenza per creare
        /// l'oggetto generics.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        internal static PropertyLinkMonitor<T, T> OnObject<T>(T source)
            where T : BaseViewModel
        {
            return new PropertyLinkMonitor<T, T>(source, source);
        }

        /// <summary>
        /// Called when [object].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">La sorgente, ovvero l'oggetto da cui partiranno
        /// gli eventi di <see cref="INotifyPropertyChanged"/> in risposta ad alcune proprietà
        /// di <paramref name="originator"/></param>
        /// <param name="originator">L'oggetto che viene monitorato, in sostanza io faccio un link
        /// per cui quando alcune proprietà di originator cambiano, io rilancio un notifypropertychanged dal
        /// <paramref name="source"/>.</param>
        /// <returns></returns>
        internal static PropertyLinkMonitor<T, O> OnObject<T, O>(T source, O originator)
            where T : BaseViewModel
            where O : class, INotifyPropertyChanged
        {
            return new PropertyLinkMonitor<T, O>(source, originator);
        }
    }
}
