using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Tests.TestInfrastructure
{
    public static class GenericEquals
    {
        private static String[] propertyBlackList = new [] { "Originator" };

        public static Boolean Equals(Object a, Object b) 
        {
            if (a == null || b == null) 
            {
                return b == null && a == null;
            }

            if (a.GetType() != b.GetType()) return false;
            var propA = a.GetType().GetProperties();
            var propB = b.GetType().GetProperties();

            if (propA.Length != propB.Length) return false;

            return propA.All(p => {

                if (propertyBlackList.Contains(p.Name)) return true;

                var pinfo = propB.SingleOrDefault(pb => pb.Name == p.Name);
                if (pinfo == null) return false;

                var bvalue = pinfo.GetValue(b, null);
                var avalue = p.GetValue(a, null);
                return Object.Equals(avalue, bvalue);
            });
        }
    }
}
