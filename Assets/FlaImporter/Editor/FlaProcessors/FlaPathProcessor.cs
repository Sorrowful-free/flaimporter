using Assets.FlaImporter.Editor.EditorCoroutine;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaPathProcessor
    {
        public static void ProcessPath(string path)
        {
            if (path.ToLower().EndsWith(".fla"))
            {
                FlaPreProcessor.ProcessZipFile(path).StartAsEditorCoroutine();
                return;
            }
            else if (path.ToLower().EndsWith(".xml"))
            {
                FlaPreProcessor.ProcessXMLFile(path).StartAsEditorCoroutine();
                return;
            }
            Debug.Log("it is no flash file");
        }
    }
}
