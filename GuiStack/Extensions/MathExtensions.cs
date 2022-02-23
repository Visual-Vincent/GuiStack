using System;

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
    }
}
