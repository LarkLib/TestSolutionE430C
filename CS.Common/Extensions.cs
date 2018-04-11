using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace LarkLib.Common.Utilities
{
    public static class Extensions
    {
        #region String

        public static bool IsNullOrEmpty(this string src)
        {
            return String.IsNullOrEmpty(src);
        }

        public static string FormatInvariantCulture(this string src, params object[] values)
        {
            if (src.IsNullOrEmpty())
            {
                throw new ArgumentNullException("src");
            }
            return String.Format(CultureInfo.InvariantCulture, src, values);
        }

        public static bool IsNullOrWhiteSpace(this string src)
        {
            return String.IsNullOrWhiteSpace(src);
        }

        public static string TrimIfNotNullOrWhiteSpace(this string src)
        {
            return !src.IsNullOrWhiteSpace() ? src.Trim() : null;
        }

        public static int Length(this string src)
        {
            return !src.IsNullOrWhiteSpace() ? src.Trim().Length : 0;
        }

        public static string Left(this string src, int offset, bool isTrimString = true)
        {
            return src.IsNullOrEmpty() ? null : (isTrimString ? src.Trim() : src).Substring(0, offset);
        }
        public static string Right(this string src, int offset, bool isTrimString = true)
        {
            var str = isTrimString ? src.Trim() : src;
            return src.IsNullOrEmpty() ? null : str.Substring(str.Length - offset);
        }
        public static decimal ToDecimal(this string src)
        {
            decimal value = default(decimal);
            return decimal.TryParse(src, out value) ? value : default(decimal);
        }
        public static int ToInt(this string src)
        {
            int value = default(int);
            return int.TryParse(src, out value) ? value : default(int);
        }
        #endregion String

        #region Enumerable

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> instance)
        {
            return ((instance == null) || (!instance.Any()));
        }

        #endregion Enumerable

        #region Double
        public static double ToCtpPrice(this double src)
        {
            if (src == double.MaxValue)
            {
                return src / 10d;
            }
            src = Math.Round(src, 1, MidpointRounding.AwayFromZero);
            return (Convert.ToInt64(src * 10) % 2) == 0 ? src : src + 0.1d;
        }

        #endregion

        #region DateTime
        public static string ToDetailString(this DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm ss.ffffff");
        }
        public static string ToNormalString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm ss.ffffff");
        }
        public static int GetIntTotalMilliseconds(this TimeSpan time)
        {
            return Convert.ToInt32(time.TotalMilliseconds);
        }
        public static int GetQuarter(this DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }

        public static int GetQuarter(this DateTimeOffset date)
        {
            return date.DateTime.GetQuarter();
        }


        #endregion

    }
}
