using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.EditorCoroutine;
using Assets.FlaImporter.Editor.Extentions;
using Assets.FlaImporter.Editor.Utils;
using Ionic.Zip;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaPreProcessor
    {
        public static IEnumerator ProcessXMLFile(string path)
        {
            var stringXML = File.ReadAllText(path);
            var data = File.ReadAllBytes(path);
            if (stringXML.StartsWith("<DOMDocument"))
            {
                ProgressLog("parse xml", "parse xml file", 0.0f);
                var flaDocument = data.ObjectFromXML<FlaDocumentRaw>();
                ProgressLog("parse xml", "parsed xml document", 0.1f);

                foreach (var includeBitmap in flaDocument.IncludeBitmaps)
                {
                    if (!includeBitmap.Href.ToLower().EndsWith(".png") && !includeBitmap.Href.ToLower().EndsWith(".jpg"))
                    {
                        Debug.Log("can't load " + includeBitmap.Href);
                        continue;
                    }

                    var filePath = FolderAndFileUtils.GetDirectoryPath(path) + "/LIBRARY/" + includeBitmap.Href;
                    AssetDataBaseUtility.CopyAndSaveBitmap(filePath,includeBitmap.Href);
                    ProgressLog("parse bitmap", "import bitmap include", (float)flaDocument.IncludeBitmaps.IndexOf(includeBitmap) / (float)flaDocument.IncludeBitmaps.Count);
                    yield return null;

                }
                ProgressLog("parse fla", "parsed fla bitmap include", 0.3f);
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
                    ProgressLog("parse symbols", "import fla symbols", (float)flaSymbols.IndexOf(flaSymbol) / (float)flaSymbols.Count);
                    yield return null;
                }

                flaSymbols = GetDependetSymbols(flaSymbols);
                ProgressLog("parse fla", "parsed fla symbols include", 0.6f);

                foreach (var flaSymbol in flaSymbols)
                {
                    yield return FlaProcessor.ProcessFlaSymbol(flaSymbol).StartAsEditorCoroutine();
                    ProgressLog("process symbols", "process fla symbols", (float)flaSymbols.IndexOf(flaSymbol) / (float)flaSymbols.Count);
                    yield return null;
                }

                ProgressLog("parse fla", "process fla symbols", 0.9f);

                yield return FlaProcessor.ProcessFlaDocument(flaDocument).StartAsEditorCoroutine();

                ProgressLog("parse fla", "process fla document", 1f);
                yield return null;
                EditorUtility.ClearProgressBar();
            }
            else if (stringXML.StartsWith("<DOMSymbolItem"))
            {
                ProgressLog("parse xml", "process xml symbol", 1f);
                var flaSymbol = data.ObjectFromXML<FlaSymbolItemRaw>();
                yield return FlaProcessor.ProcessFlaSymbol(flaSymbol).StartAsEditorCoroutine();
                EditorUtility.ClearProgressBar();
            }
            yield return null;
        }

        public static IEnumerator ProcessZipFile(string path)
        {
            ProgressLog("parse fla", "parse fla file", 0.0f);

            var flaFile = ZipFile.Read(path);
            var document = flaFile["DOMDocument.xml"];

            var flaDocument = document.ToByteArray().ObjectFromXML<FlaDocumentRaw>();
            ProgressLog("parse fla", "parsed fla document", 0.1f);

            foreach (var includeBitmap in flaDocument.IncludeBitmaps)
            {
                if (!includeBitmap.Href.ToLower().EndsWith(".png") && !includeBitmap.Href.ToLower().EndsWith(".jpg"))
                {
                    Debug.Log("can't load " + includeBitmap.Href);
                    continue;
                }
                var zipFileEntry = flaFile.FirstOrDefault(e => e.FileName.EndsWith(includeBitmap.Href));
                AssetDataBaseUtility.SaveBitmapTo(zipFileEntry.ToByteArray(), includeBitmap.Href);
                ProgressLog("parse bitmap", "import bitmap include", (float)flaDocument.IncludeBitmaps.IndexOf(includeBitmap) / (float)flaDocument.IncludeBitmaps.Count);
                yield return null;

            }
            ProgressLog("parse fla", "parsed fla bitmap include", 0.3f);


            var flaSymbols = new List<FlaSymbolItemRaw>();

            foreach (var includeSymbol in flaDocument.IncludeSymbols)
            {
                var zipFileEntry = flaFile.FirstOrDefault(e => e.FileName.EndsWith(includeSymbol.Href));
                var flaSymbol = zipFileEntry.ToByteArray().ObjectFromXML<FlaSymbolItemRaw>();
                flaSymbols.Add(flaSymbol);
                ProgressLog("parse symbols", "import fla symbols", (float)flaSymbols.IndexOf(flaSymbol) / (float)flaSymbols.Count);
                yield return null;
            }
            ProgressLog("parse fla", "parsed fla symbols include", 0.6f);

            flaSymbols = GetDependetSymbols(flaSymbols);
            foreach (var flaSymbol in flaSymbols)
            {
                yield return FlaProcessor.ProcessFlaSymbol(flaSymbol).StartAsEditorCoroutine();
                ProgressLog("process symbols", "process fla symbols", (float)flaSymbols.IndexOf(flaSymbol) / (float)flaSymbols.Count);
                yield return null;
            }
            ProgressLog("parse fla", "process fla symbols", 0.9f);

            yield return FlaProcessor.ProcessFlaDocument(flaDocument).StartAsEditorCoroutine();

            ProgressLog("parse fla", "process fla document", 1f);
            yield return null;
            EditorUtility.ClearProgressBar();
            // EditorUtility.ClearProgressBar();
        }

        private static List<FlaSymbolItemRaw> GetDependetSymbols(List<FlaSymbolItemRaw> symbols)
        {
            var dependents = symbols.OrderBy(e => GetDependensDepth(e, symbols, 0));
            return dependents.ToList();
        }

        private static int GetDependensDepth(FlaSymbolItemRaw symbol, List<FlaSymbolItemRaw> symbols, int depth)
        {
            if (symbol == null || symbol.Timeline == null || symbol.Timeline.Timeline == null)
            {
                return depth;
            }
            var layers = symbol.Timeline.Timeline.Layers;
            var includeSymbols = layers.SelectMany(l => l.Frames.SelectMany(f => f.Elements)).Where(e => e is FlaBaseInstanceRaw).Select(e => e as FlaBaseInstanceRaw);
            var maxDepth = depth;
            foreach (var elementRaw in includeSymbols)
            {
                var includeSumbol = symbols.FirstOrDefault(e => e.Name == elementRaw.LibraryItemName);
                maxDepth = Math.Max(maxDepth, GetDependensDepth(includeSumbol, symbols, depth + 1));
            }
            return maxDepth;
        }

        private static void ProgressLog(string title, string text, float percents)
        {
            EditorUtility.DisplayProgressBar(title, text, percents);
            //  Debug.Log(string.Format("{0}.{1}({2}%)", title, text, percents * 100.0f));
        }

    }
}
