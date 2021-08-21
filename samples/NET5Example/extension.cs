using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NET5Example
{
    public static class extension
    {
        public static int RegexIndexOf(this string str, string pattern)
        {
            var m = Regex.Match(str, pattern);
            return m.Success ? m.Index : -1;
        }
    }
}
