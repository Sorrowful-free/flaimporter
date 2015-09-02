using System.IO;
using System.Linq;
using Assets.FlaExporter.Editor.Extentions;
using Assets.FlaExporter.Editor.Utils;
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

        
        private const string BitmapSymbolsTextureFolderFolder = "/BitmapSymbols/Textures/";
        

        private void OnGUI()
        {
            if (GUILayout.Button("OpenFLA"))
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
                ProcessZipFile(path);
                return;
            }
            else if (path.ToLower().EndsWith(".xml"))
            {
                ProcessXMLFile(path);
                return;
            }
            Debug.Log("it is no flash file");
        }

        
        private void ProcessXMLFile(string path)
        {
            var stringXML = File.ReadAllText(path);
            var data = File.ReadAllBytes(path);
            if (stringXML.StartsWith("<DOMDocument"))
            {
                var flaDocument = FlaParser.ProcessDocumentXml(data);

                foreach (var includeSymbol in flaDocument.IncludeSymbols)
                {
                    var filePath = FolderAndFileUtils.GetDirectoryPath(path) + "/LIBRARY/" + includeSymbol.Href;
                    if (!File.Exists(filePath))
                    {
                        Debug.Log("file not found " + filePath);
                        continue;
                    }
                    var flaSymbol = FlaParser.ProcessSymbolXml(File.ReadAllBytes(filePath));
                    FlaProcessor.ProcessFlaSymbol(flaSymbol);
                }
                foreach (var includeBitmap in flaDocument.IncludeBitmaps)
                {
                    if (!includeBitmap.Href.ToLower().EndsWith(".png") && !includeBitmap.Href.ToLower().EndsWith(".jpg"))
                    {
                        Debug.Log("can't load " + includeBitmap.Href);
                        continue;
                    }

                    var filePath = FolderAndFileUtils.GetDirectoryPath(path) + "/LIBRARY/" + includeBitmap.Href;
                    if (!File.Exists(filePath))
                    {
                        Debug.Log("file not found " + filePath);
                        continue;
                    }
                    FolderAndFileUtils.CheckFolders(BitmapSymbolsTextureFolderFolder);
                    File.Copy(filePath, Application.dataPath + BitmapSymbolsTextureFolderFolder + includeBitmap.Href);
                    AssetDatabase.ImportAsset("Assets"+BitmapSymbolsTextureFolderFolder + includeBitmap.Href);
                    AssetDatabase.Refresh();
                    FlaProcessor.ProcessFlaBitmapSymbol(includeBitmap);
                }
                

                FlaProcessor.ProcessFlaDocument(flaDocument);
            }
            else if (stringXML.StartsWith("<DOMSymbolItem"))
            {
                var flaSymbol = FlaParser.ProcessSymbolXml(data);
                FlaProcessor.ProcessFlaSymbol(flaSymbol);
            }
        }

        private void ProcessZipFile(string path)
        {
            var flaFile = ZipFile.Read(path);
            var document = flaFile["DOMDocument.xml"];

            var flaDocument = FlaParser.ProcessDocumentXml(document.ToByteArray());
            foreach (var includeSymbol in flaDocument.IncludeSymbols)
            {
                var zipFileEntry = flaFile.FirstOrDefault(e => e.FileName.EndsWith(includeSymbol.Href));
                var flaSymbol = FlaParser.ProcessSymbolXml(zipFileEntry.ToByteArray());
                FlaProcessor.ProcessFlaSymbol(flaSymbol);
            }
            foreach (var includeBitmap in flaDocument.IncludeBitmaps)
            {
                if (!includeBitmap.Href.ToLower().EndsWith(".png") && !includeBitmap.Href.ToLower().EndsWith(".jpg"))
                {
                    Debug.Log("can't load " + includeBitmap.Href);
                    continue;
                }
                var zipFileEntry = flaFile.FirstOrDefault(e => e.FileName.EndsWith(includeBitmap.Href));

                FolderAndFileUtils.CheckFolders(BitmapSymbolsTextureFolderFolder);
                var file = File.Open(Application.dataPath + BitmapSymbolsTextureFolderFolder + includeBitmap.Href, FileMode.OpenOrCreate);
                var bytes = zipFileEntry.ToByteArray();
                file.Write(bytes, 0, bytes.Length);
                file.Close();
                AssetDatabase.ImportAsset("Assets" + BitmapSymbolsTextureFolderFolder + includeBitmap.Href);
                AssetDatabase.Refresh();
                FlaProcessor.ProcessFlaBitmapSymbol(includeBitmap);
                
            }
            
            FlaProcessor.ProcessFlaDocument(flaDocument);
        }

        


        

        

    }
}
