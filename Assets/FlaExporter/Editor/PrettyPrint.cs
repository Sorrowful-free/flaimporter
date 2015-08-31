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
            return PrintObj(obj,0);
        }

        private static string PrintObj(object obj,int depth)
        {
            var separator = "";
            for (int i = 0; i < depth; i++)
            {
                separator += "-";
            }
            if (obj is string)
            {
                return obj as string;
            }
            if (obj is IEnumerable)
            {
                return separator+ PrintIEnumerable(obj as IEnumerable, depth);
            }
            if (obj == null)
            {
                return separator+"null";
            }
            var resultString = "\n";
            
            var objType = obj.GetType();
            var props = objType.GetProperties();
            var fields = objType.GetFields();
            if (props.Length > 0)
                resultString += props.Select(e => separator + (e.PropertyType.IsValueType ?  e.Name + ":" + e.GetValue(obj, null) : e.Name + ":" + PrintObj(e.GetValue(obj, null), depth + 1))).JoinToString("\n");

            if (fields.Length > 0)
                resultString += fields.Select(e => separator + (e.FieldType.IsValueType ? e.Name + ":" + e.GetValue(obj) : e.Name + ":" + PrintObj(e.GetValue(obj), depth + 1))).JoinToString("\n");
            //if(props.Length>0)
            //resultString += props.Select(e =>
            //            e.PropertyType.IsValueType
            //                ? e.Name + ":" + e.GetValue(obj, null)
            //                : typeof (IEnumerable).IsAssignableFrom(e.PropertyType) && e.PropertyType != typeof(string)
            //                    ? e.Name + ":" + PrintIEnumerable(e.GetValue(obj, null) as IEnumerable)
            //                    : e.Name + ":" + PrintObj(e.GetValue(obj, null))).JoinToString("\n");
            //if (fields.Length > 0)
            //resultString += fields.Select(e =>
            //            e.FieldType.IsValueType
            //                ? e.Name + ":" + e.GetValue(obj)
            //                : typeof(IEnumerable).IsAssignableFrom(e.FieldType) && e.FieldType != typeof(string)
            //                    ? e.Name + ":" + PrintIEnumerable(e.GetValue(obj) as IEnumerable)
            //                    : e.Name + ":" + PrintObj(e.GetValue(obj))).JoinToString("\n");
            return resultString;
        }
        
        private static string PrintIEnumerable(IEnumerable enumerable,int depth)
        {
            if (enumerable == null)
            {
                return "null";
            }
           
           
            var result = "";
            
            foreach (var element in enumerable)
            {
                result += PrintObj(element, depth ) + "\n";
            }
            return result;
        }
    }
}
