using System.Collections;
using System.Linq;

namespace Assets.FlaExporter.Editor.Extentions
{
    public static class PrettyPrintExtention 
    {
        public static string PrettyPrintObjects(this object obj)
        {
            return PrintObj(obj,0);
        }

        private static string PrintObj(object obj,int depth)
        {
           
            if (obj is string)
            {
                return obj as string + "\n";
            }
            if (obj is IEnumerable)
            {
                return PrintIEnumerable(obj as IEnumerable, depth);
            }
            if (obj == null)
            {
                return "null\n";
            }
            var resultString = "";
            var separator = "";
            for (int i = 0; i < depth; i++)
            {
                separator += "-";
            }
            if (depth > 0)
                resultString += "\n";
            var objType = obj.GetType();
            var props = objType.GetProperties();
            var fields = objType.GetFields();
            if (props.Length > 0)
                resultString += props.Select(e => (e.PropertyType.IsValueType ? separator + e.Name + ":" + e.GetValue(obj, null) +"\n": separator + e.Name + ":" + PrintObj(e.GetValue(obj, null), depth + 1))).JoinToString("");

            if (fields.Length > 0)
                resultString += fields.Select(e => (e.FieldType.IsValueType ? separator + e.Name + ":" + e.GetValue(obj) + "\n" : separator + e.Name + ":" + PrintObj(e.GetValue(obj), depth + 1))).JoinToString("");
            
            return resultString + "\n";
        }
        
        private static string PrintIEnumerable(IEnumerable enumerable,int depth)
        {
            if (enumerable == null)
            {
                return "null";
            }

            var separator = "";
            for (int i = 0; i < depth; i++)
            {
                separator += "-";
            }
            var result = "\n";
            
            foreach (var element in enumerable)
            {
                result += "\n" + separator+"+";
                result += PrintObj(element, depth );
            }
            return result;
        }
    }
}
