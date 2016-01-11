using System;
using System.Collections.Generic;

namespace Assets.BundleExporter.Editor.Helpers
{
    public static class StringExtention
    {
        internal static string JoinToString<TObject>(this IEnumerable<TObject> enumerable, string separator)
        {
            string result = String.Empty;
            foreach (var item in enumerable)
            {
                result += item + separator;
            }
            if(result.Length >= 1)
                result = result.Substring(0,result.Length-1);
            return result;
        }

    }
}
