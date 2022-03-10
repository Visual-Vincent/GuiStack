using System;
using System.Collections.Generic;

namespace GuiStack.Extensions
{
    public static class MathExtensions
    {
        private static readonly string[] Suffixes = new string[] { " B", " KB", " MB", " GB", " TB", " PB" };

        /// <summary>
        /// Converts the number into a human-readable value with a file size suffix (B, KB, MB, GB, TB, PB).
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="decimals">The number of decimals to display. Defaults to 1.</param>
        public static string ToFormattedFileSize(this long value, int decimals = 1)
        {
            int i;
            var rounding = MidpointRounding.AwayFromZero;

            for(i = 0; i < Suffixes.Length - 1; i++)
            {
                if(value < Math.Pow(1024, i+1))
                    return Math.Round(value / Math.Pow(1024, i), decimals, rounding) + Suffixes[i];
            }

            return Math.Round(value / Math.Pow(1024, i), decimals, rounding) + Suffixes[i];
        }

        /// <summary>
        /// Converts the number into a human-readable value with a file size suffix (B, KB, MB, GB, TB, PB).
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="decimals">The number of decimals to display. Defaults to 1.</param>
        public static string ToFormattedFileSize(this int value, int decimals = 1)
        {
            return ((long)value).ToFormattedFileSize(decimals);
        }

        /// <summary>
        /// Formats the <see cref="TimeSpan"/> as a human-readable string.
        /// </summary>
        /// <param name="span">The <see cref="TimeSpan"/> to format.</param>
        /// <param name="shortFormat">Optional. Whether or not to use a short or long time format.</param>
        public static string ToFormattedString(this TimeSpan span, bool shortFormat = false)
        {
            List<string> parts = new List<string>();

            string FormatValue(int value, string suffix, bool pluralize = true)
            {
                if(value == 1 || !pluralize)
                    return $"{value}{suffix}";

                return $"{value}{suffix}s";
            }

            if(!shortFormat)
            {
                if(span.Days > 0) parts.Add(FormatValue(span.Days, " day"));
                if(span.Hours > 0) parts.Add(FormatValue(span.Days, " hour"));
                if(span.Minutes > 0) parts.Add(FormatValue(span.Days, " minute"));
                if(span.Seconds > 0) parts.Add(FormatValue(span.Days, " second"));
                if(span.Milliseconds > 0) parts.Add(FormatValue(span.Days, " millisecond"));
            }
            else
            {
                if(span.Days > 0) parts.Add(FormatValue(span.Days, "d", false));
                if(span.Hours > 0) parts.Add(FormatValue(span.Days, "h", false));
                if(span.Minutes > 0) parts.Add(FormatValue(span.Days, " min", false));
                if(span.Seconds > 0) parts.Add(FormatValue(span.Days, " sec", false));
                if(span.Milliseconds > 0) parts.Add(FormatValue(span.Days, "ms", false));
            }

            if(parts.Count <= 0)
                if(!shortFormat)
                    return "0 seconds";
                else
                    return "0 sec";

            return string.Join(" ", parts);
        }
    }
}
