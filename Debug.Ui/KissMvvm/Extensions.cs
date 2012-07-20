using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Sample.DebugUi.KissMvvm
{
    public static class Extensions
    {
        public static void Set<T, U>(this T instance, Expression<Func<T, U>> action, U newValue, ref U backingField)
            where T : BaseViewModel
        {
            if ((backingField == null) && (newValue == null)) return;
            if ((newValue != null) && (newValue.Equals(backingField))) return;
            String name = action.GetMemberName();
            backingField = newValue;
            instance.RaisePropertyChanged(name);
        }
    }
}
