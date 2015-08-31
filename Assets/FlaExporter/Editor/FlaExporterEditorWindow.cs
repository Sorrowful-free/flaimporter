using System.IO;
using Assets.BundleExporter.Editor.Helpers;
using Assets.FlaExporter.Data.RawData;
using Assets.Scripts.Helpers.Extensions;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.Editor
{
    public class FlaExporterEditorWindow : EditorWindow
    {
        [MenuItem("FlaExporter/FlaExporter")]
        public static void ShowWindow()
        {
            var window = CreateInstance<FlaExporterEditorWindow>();
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("OpenDoc"))
            {
                var path = EditorUtility.OpenFilePanel("fla", "Assets", "xml");
                var data = File.ReadAllBytes(path);
                //var br = new BinaryReader(new MemoryStream(data));
               // Debug.Log(br.ReadString() + br.ReadString() + br.ReadString());
                var fla = data.ObjectFromXML<FlaDocumentRaw>();
                Debug.Log(fla+"\n"+fla.Timelines.JoinToString("\n"));
                foreach (var timeLineRaw in fla.Timelines)
                {
                    Debug.Log("ololo");
                    foreach (var layer in timeLineRaw.Layers)
                    {
                        Debug.Log("layer:"+layer.ToString());
                    }
                }
            }
        }
    }
}
