using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var fla = data.ObjectFromXML<FlaDocumentRaw>();
                Debug.Log(fla.PrettyPrintObjects());
            }
        }
    }
}
