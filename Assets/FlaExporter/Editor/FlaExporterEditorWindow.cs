using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.FlaExporter.Data.RawData;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.Extentions;
using Assets.FlaExporter.Editor.Utils;
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
                var flaDocument = data.ObjectFromXML<FlaDocumentRaw>();

               
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
                    FolderAndFileUtils.CheckFolders(FoldersConstants.BitmapSymbolsTextureFolderFolder);
                    File.Copy(filePath, Application.dataPath + FoldersConstants.BitmapSymbolsTextureFolderFolder + includeBitmap.Href);
                    AssetDatabase.ImportAsset(FolderAndFileUtils.GetAssetFolder( FoldersConstants.BitmapSymbolsTextureFolderFolder) + includeBitmap.Href);
                    AssetDatabase.Refresh();
                }

                var flaSymbols = new List<FlaSymbolItemRaw>();
                foreach (var includeSymbol in flaDocument.IncludeSymbols)
                {
                    var filePath = FolderAndFileUtils.GetDirectoryPath(path) + "/LIBRARY/" + includeSymbol.Href;
                    if (!File.Exists(filePath))
                    {
                        Debug.Log("file not found " + filePath);
                        continue;
                    }
                    var flaSymbol = File.ReadAllBytes(filePath).ObjectFromXML<FlaSymbolItemRaw>();
                    flaSymbols.Add(flaSymbol);
                }

                flaSymbols = GetDependetSymbols(flaSymbols);
                Debug.Log("symbols count = "+flaSymbols.Count);

                foreach (var flaSymbol in flaSymbols)
                {
                    FlaProcessor.ProcessFlaSymbol(flaSymbol);
                }

                FlaProcessor.ProcessFlaDocument(flaDocument);
            }
            else if (stringXML.StartsWith("<DOMSymbolItem"))
            {
                var flaSymbol = data.ObjectFromXML<FlaSymbolItemRaw>();
                FlaProcessor.ProcessFlaSymbol(flaSymbol);
            }
        }

        private void ProcessZipFile(string path)
        {
            var flaFile = ZipFile.Read(path);
            var document = flaFile["DOMDocument.xml"];

            var flaDocument = document.ToByteArray().ObjectFromXML<FlaDocumentRaw>();
           
            foreach (var includeBitmap in flaDocument.IncludeBitmaps)
            {
                if (!includeBitmap.Href.ToLower().EndsWith(".png") && !includeBitmap.Href.ToLower().EndsWith(".jpg"))
                {
                    Debug.Log("can't load " + includeBitmap.Href);
                    continue;
                }
                var zipFileEntry = flaFile.FirstOrDefault(e => e.FileName.EndsWith(includeBitmap.Href));

                FolderAndFileUtils.CheckFolders(FoldersConstants.BitmapSymbolsTextureFolderFolder);
                var file = File.Open(Application.dataPath + FoldersConstants.BitmapSymbolsTextureFolderFolder + includeBitmap.Href, FileMode.OpenOrCreate);
                var bytes = zipFileEntry.ToByteArray();
                file.Write(bytes, 0, bytes.Length);
                file.Close();
                AssetDatabase.ImportAsset(FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsTextureFolderFolder) + includeBitmap.Href);
                AssetDatabase.Refresh();
            }

            var flaSymbols = new List<FlaSymbolItemRaw>();

            foreach (var includeSymbol in flaDocument.IncludeSymbols)
            {
                var zipFileEntry = flaFile.FirstOrDefault(e => e.FileName.EndsWith(includeSymbol.Href));
                var flaSymbol = zipFileEntry.ToByteArray().ObjectFromXML<FlaSymbolItemRaw>();
                flaSymbols.Add(flaSymbol);
                //FlaProcessor.ProcessFlaSymbol(flaSymbol);
            }

            flaSymbols = GetDependetSymbols(flaSymbols);

            foreach (var flaSymbol in flaSymbols)
            {
                FlaProcessor.ProcessFlaSymbol(flaSymbol);
            }

            FlaProcessor.ProcessFlaDocument(flaDocument);
        }

        private List<FlaSymbolItemRaw> GetDependetSymbols(List<FlaSymbolItemRaw> symbols)
        {
            var dependents = symbols.OrderBy(e => GetDependensDepth(e, symbols, 0));
            return dependents.ToList();
        }

        private int GetDependensDepth(FlaSymbolItemRaw symbol,List<FlaSymbolItemRaw> symbols,int depth)
        {
            if (symbol == null || symbol.Timeline == null || symbol.Timeline.Timeline == null)
            {
                return depth;
            }
            var layers = symbol.Timeline.Timeline.Layers;
            var includeSymbols = layers.SelectMany(l => l.Frames.SelectMany(f => f.Elements)).Where(e => e is FlaBaseInstanceRaw).Select(e=>e as FlaBaseInstanceRaw);
            var maxDepth = depth;
            foreach (var elementRaw in includeSymbols)
            {
                var includeSumbol = symbols.FirstOrDefault(e => e.Name == elementRaw.LibraryItemName);
                maxDepth = Math.Max(maxDepth, GetDependensDepth(includeSumbol, symbols, depth + 1));
            }
            return maxDepth;
        }


        

        

    }
}
