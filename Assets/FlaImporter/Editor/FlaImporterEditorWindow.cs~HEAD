using Assets.FlaImporter.Editor.EditorCoroutine;
using Assets.FlaImporter.Editor.FlaProcessors;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaImporter.Editor
{
    public class FlaImporterEditorWindow : EditorWindow
    {
        private static FlaImporterEditorWindow _instance;
        [MenuItem("FlaImporter/FlaImporter")]
        public static void ShowWindow()
        {
            if (_instance == null)
            {
                _instance = CreateInstance<FlaImporterEditorWindow>();
                _instance.titleContent = new GUIContent("FlaImporter");
            }
            _instance.Show();
            _instance.Focus();
        }
        
        private void OnGUI()
        {
            if (GUILayout.Button("Open FLA"))
            {
                var path = EditorUtility.OpenFilePanel("open flash file (fla)", "Assets","fla");
                if (path == "")
                {
                    return;
                }
                ProcessPath(path);
            }
            if (GUILayout.Button("Open XFL (xml)"))
            {
                var path = EditorUtility.OpenFilePanel("open flash file (xml)", "Assets", "xml");
                if (path == "")
                {
                    return;
                }
                ProcessPath(path);
            }
        }
        
        private void ProcessPath(string path)
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
