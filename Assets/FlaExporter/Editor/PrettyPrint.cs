namespace Assets.FlaExporter.Editor
{
    public static class PrettyPrint 
    {
        public static void PrettyPrintObjects(this object obj)
        {
            var resultString = "";
            var objType = obj.GetType();
            var props = objType.GetProperties();
            var fields = objType.GetFields();
        }

        private static string PrintObj(object obj)
        {
            return "";
        }
    }
}
