using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Replace(this string Str, char[] separators, string newStr)
        {
            var separatorsHashset = new HashSet<char>(separators);
            var strBuilder = new StringBuilder();
            foreach(var ch in newStr)
            {
                if (separatorsHashset.Contains(ch))
                    strBuilder.Append(newStr);
                else
                    strBuilder.Append(ch);
            }
            return strBuilder.ToString();
        }

        public static string RemoveNewlines(this string Str)
        {
            return Str.Replace(new char[] { '\r', '\n' }, String.Empty);
        }
    }
}
