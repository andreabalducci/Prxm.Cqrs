using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.QueryModel.NHibernate
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DenormalizerVersionAttribute : Attribute
    {
        public Int32 Version { get; set; }

        public DenormalizerVersionAttribute(Int32 currentValue)
        {
            Version = currentValue;
        }
    }
}
