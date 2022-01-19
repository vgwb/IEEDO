using System.Collections.Generic;
using System.Text;

namespace Ieedo.Utilities
{
    public static class StringUtilities
    {
        public static string ToJoinedString<T>(this IEnumerable<T> list)
        {
            var s = new StringBuilder();
            foreach (var v in list)
                s.AppendLine($" {v}");
            return s.ToString();
        }
    }
}