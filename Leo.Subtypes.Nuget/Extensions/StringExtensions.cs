using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Subtypes.Extensions
{
    public static class StringExtensions
    {
        public static int ConvertVersionToNumber(this string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return 0;

            try
            {
                return int.Parse(version.Replace(".", ""));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to get version number from " + version, ex);
                return 0;
            }
        }
    }
}