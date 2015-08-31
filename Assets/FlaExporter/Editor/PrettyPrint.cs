using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.BundleExporter.Editor.Helpers;
using Debug = UnityEngine.Debug;

namespace Assets.FlaExporter.Editor
{
    public static class PrettyPrint 
    {
        public static string PrettyPrintObjects(this object obj)
        {
            return PrintObj(obj);
        }

        private static string PrintObj(object obj)
        {
            var resultString = "";
            var objType = obj.GetType();
            var props = objType.GetProperties();
            var fields = objType.GetFields();// typeOfBaseClass.IsAssignableFrom(t)
            resultString = props.Select(e => typeof(IEnumerable).IsAssignableFrom(e.PropertyType) ? e.Name + ":" + PrintIEnumerable(e.GetValue(obj, null) as IEnumerable) : e.Name + ":" + e.GetValue(obj, null)).JoinToString("\n");
            resultString += fields.Select(e => typeof(IEnumerable).IsAssignableFrom(e.FieldType) ? e.Name + ":" + PrintIEnumerable(e.GetValue(obj) as IEnumerable) : e.Name + ":" + e.GetValue(obj)).JoinToString("\n");

            return resultString;
        }

        private static string PrintIEnumerable(IEnumerable enumerable)
        {
            Debug.Log("enum");
            if (enumerable == null)
            {
                return"null";
            }
            var result = "";
            foreach (var element in enumerable)
            {
                result+=PrintObj(element)+"\n";
            }
            return result;
        }
    }
}
