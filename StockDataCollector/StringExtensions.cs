using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.ComponentModel;

namespace StockDataCollector
{
    public static class StringFormatExtensions
    {
        /// <summary>
        /// To see if a string is null or empty string
        /// </summary>
        /// <param name="src">the string</param>
        /// <returns>bool</returns>
        public static bool IsNullOrEmpty(this string src)
        {
            return String.IsNullOrEmpty(src);
        }

        /// <summary>
        /// wrapper of string.format
        /// </summary>
        /// <param name="src">the string</param>
        /// <param name="values">format params</param>
        /// <returns>bool</returns>
        public static string FormatInvariantCulture(this string src, params object[] values)
        {
            if (src.IsNullOrEmpty())
            {
                throw new ArgumentNullException("src");
            }
            return String.Format(CultureInfo.InvariantCulture, src, values);
        }

        /// <summary>
        /// To see if a string is null or white space
        /// </summary>
        /// <param name="src">the string</param>
        /// <returns>bool</returns>
        public static bool IsNullOrWhiteSpace(this string src)
        {
            return String.IsNullOrWhiteSpace(src);
        }
    }
}
