using System;
using System.Collections.Generic;

namespace Assets.FlaImporter.Editor.Extentions
{
    public static class StringExtention
    {
        internal static string JoinToString<TObject>(this IEnumerable<TObject> enumerable, string separator)
        {
            string result = String.Empty;
            foreach (var item in enumerable)
            {
                result += (item == null?"null":item.ToString()) + separator;
            }
            if(result.Length >= 1)
                result = result.Substring(0,result.Length-1);
            return result;
        }

    }
}
