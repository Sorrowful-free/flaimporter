using System.IO;
using Assets.FlaExporter.Data.RawData;
using Assets.Scripts.Helpers.Extensions;
using Ionic.Zip;
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
                var path = EditorUtility.OpenFilePanel("open flash file (fla;xml)", "Assets","*.*");
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
                ProcessZip(path);
                return;
            }
            else if (path.ToLower().EndsWith(".xml"))
            {
                ProcessXMLFile(path);
                return;
            }
            Debug.Log("it is no flash file");
        }

        private void ProcessZip(string path)
        {
            var flaFile = ZipFile.Read(path);
            foreach (var entry in flaFile)
            {
                Debug.Log(entry.FileName);
            }
            var document = flaFile["DOMDocument.xml"];
            var writeSpace = new MemoryStream();
            writeSpace.Position = 0;

            var reader = document.OpenReader();
            var buffer = new byte[2048];
            var len = 0;
            while ((len = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                writeSpace.Write(buffer, 0, len);
            }
            ProcessDocumentXml(writeSpace.ToArray());
        }

        private void ProcessXMLFile(string path)
        {
            var stringXML = File.ReadAllText(path);
            var data = File.ReadAllBytes(path);
            if (stringXML.StartsWith("<DOMDocument"))
            {
                ProcessDocumentXml(data);
            }
            else if (stringXML.StartsWith("<DOMSymbolItem"))
            {
                ProcessSymbolXml(data); 
            }
            
        }

        private void ProcessSymbolXml(byte[] bytes)
        {
            var fla = bytes.ObjectFromXML<FlaSymbolItemRaw>();
            ProcessFlaSymbol(fla);
        }

        private void ProcessFlaSymbol(FlaSymbolItemRaw flaData)
        {
            Debug.Log(flaData.PrettyPrintObjects());
        }

        private void ProcessDocumentXml(byte[] bytes)
        {
            var fla = bytes.ObjectFromXML<FlaDocumentRaw>(); 
            ProcessFlaDocument(fla);
        }

        private void ProcessFlaDocument(FlaDocumentRaw flaData)
        {
            Debug.Log(flaData.PrettyPrintObjects());
            return;
            foreach (var timeline in flaData.Timelines)
            {
                var timelineGO = new GameObject(timeline.Name);
                foreach (var layer in timeline.Layers)
                {
                    var layerGO = new GameObject(layer.Name);
                    layerGO.transform.SetParent(timelineGO.transform);
                    foreach (var frame in layer.Frames)
                    {
                        var frameGO = new GameObject(frame.Name == null ? "frame" + frame.Index : frame.Name);
                        frameGO.transform.SetParent(layerGO.transform);
                    }
                }
            }
        }
    }
}
