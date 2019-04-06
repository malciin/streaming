using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Replace string from ceratin Index with certain Length with new string
        /// </summary>
        /// <param name="IndexStart"></param>
        /// <param name="Length"></param>
        /// <param name="NewString"></param>
        /// <returns></returns>
        public static string Replace(this string Str, int StartIndex, int Length, string NewString)
        {
            return Str.Substring(0, StartIndex) + NewString + Str.Substring(StartIndex + Length);
        }

        public static string RemoveNewlines(this string Str)
        {
            return Str.Replace(new char[] { '\r', '\n' }, String.Empty);
        }

        /// <summary>
        /// Return normalized Path for current OS
        /// </summary>
        /// <returns></returns>
        public static string NormalizePathForOS(this string Str)
            => Path.DirectorySeparatorChar == '/' ?
                Str.Replace('\\', Path.DirectorySeparatorChar) :
                Str.Replace('/', Path.DirectorySeparatorChar);


        private static Char[] base64Chars = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' };

        /// <summary>
        /// Return true if the given string is base64 encoded string. All credits goes to Qybek, check
        /// https://stackoverflow.com/a/23955827 for more details
        /// </summary>
        /// <param name="value">String to check</param>
        /// <returns>true/false</returns>
        public static bool IsBase64String(this string value)
        {
            // The quickest test. If the value is null or is equal to 0 it is not base64
            // Base64 string's length is always divisible by four, i.e. 8, 16, 20 etc. 
            // If it is not you can return false. Quite effective
            // Further, if it meets the above criterias, then test for spaces.
            // If it contains spaces, it is not base64
            if (value == null || value.Length == 0 || value.Length % 4 != 0
                || value.Contains(' ') || value.Contains('\t') || value.Contains('\r') || value.Contains('\n'))
                return false;

            // 98% of all non base64 values are invalidated by this time.
            var index = value.Length - 1;

            // if there is padding step back
            if (value[index] == '=')
                index--;

            // if there are two padding chars step back a second time
            if (value[index] == '=')
                index--;

            // Now traverse over characters
            // You should note that I'm not creating any copy of the existing strings, 
            // assuming that they may be quite large
            for (var i = 0; i <= index; i++)
                // If any of the character is not from the allowed list
                if (!base64Chars.Contains(value[i]))
                    // return false
                    return false;

            // If we got here, then the value is a valid base64 string
            return true;
        }

        /// <summary>
        /// Return substring from begin to the last occurence from the string, for example
        /// hello,world,hello with ',' give hello,world
        /// </summary>
        /// <param name="str"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string SubstringToLastOccurence(this string str, char split)
        {
            var index = str.LastIndexOf(split);
            if (index < 0)  // not found - we return original string
                return str;
            return str.Substring(0, index);
        }
    }
}
