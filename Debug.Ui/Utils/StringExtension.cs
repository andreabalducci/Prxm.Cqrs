using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.DebugUi
{
    public static class StringExtension
    {
        public static Boolean Contains(this String theString, String searchedString, StringComparison comparison)
        {
            if (theString == null) return false;
            if (String.IsNullOrEmpty(searchedString)) return true;
            return theString.IndexOf(searchedString, comparison) >= 0;
        }
    }
}
