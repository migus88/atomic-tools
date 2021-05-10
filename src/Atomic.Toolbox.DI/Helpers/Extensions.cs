using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Atomic.Toolbox.DI.Core.Enums;
using Microsoft.CodeAnalysis;

namespace Atomic.Toolbox.DI.Helpers
{
    public static class Extensions
    {
        public static string ToPascalCase(this string str)
        {
            var camelCaseStr = str.ToCamelCase();
            return $"{char.ToUpperInvariant(camelCaseStr[0])}{camelCaseStr.Substring(1)}";
        }

        public static string ToCamelCase(this string str)
        {
            var pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            return new string(
                new CultureInfo("en-US", false)
                    .TextInfo
                    .ToTitleCase(
                        string.Join(" ", pattern.Matches(str).Cast<Match>().Select(m => m.Value).ToArray()).ToLower()
                    )
                    .Replace(@" ", "")
                    .Select((x, i) => i == 0 ? char.ToLower(x) : x)
                    .ToArray()
            );
        }

        public static string ToFullString(this InitMode mode)
        {
            return $"{nameof(InitMode)}.{mode}";
        }
    }
}