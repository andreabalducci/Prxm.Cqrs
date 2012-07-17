using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.QueryModel.NHibernate
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CurrentDenormalizerVersionAttribute : Attribute
    {
        public Int32 CurrentValue { get; set; }

        public CurrentDenormalizerVersionAttribute(Int32 currentValue)
        {
            CurrentValue = currentValue;
        }
    }
}
