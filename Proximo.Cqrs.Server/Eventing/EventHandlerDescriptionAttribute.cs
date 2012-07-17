using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Eventing
{
    /// <summary>
    /// it is used to specify to the system some specific charachteristic of the
    /// Handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventHandlerDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Singleton means that only an instance of it will exist in the system
        /// </summary>
        public Boolean IsSingleton { get; set; }

        public EventHandlerDescriptionAttribute()
        {

        }

        //public EventHandlerDescriptionAttribute(Boolean isSingleton) 
        //{
        //    IsSingleton = true;
        //}
    }
}
