using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krydsord.Helpers
{
    public static class StringHelper
    {
        // Explodes if chars >= s.Length
        public static string Rotate(this string s, int chars)
        {
            return s.Substring(chars) + s.Substring(0, chars);
        }
    }
}
