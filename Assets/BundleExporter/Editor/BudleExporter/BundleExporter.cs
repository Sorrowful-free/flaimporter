using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.BundleExporter.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace Assets.BundleExporter.Editor.BudleExporter
{
    static class BundleExporter
    {

        [MenuItem("BundleExporter/ExportToAssetBundle")]
        [MenuItem("Assets/ExportToAssetBundle")]
        private static void ExportBundles() 
        {
            var prefabs = new List<string>();
            foreach (var selectedObject in Selection.objects)
            {
                if (selectedObject.GetType() == typeof (DefaultAsset))
                {
                    prefabs.AddRange(ExtractObjectsPathFromFolder(selectedObject as DefaultAsset));
                }
                else
                {
                    prefabs.Add(AssetDatabase.GetAssetPath(selectedObject));
                }
            }
            prefabs = prefabs.Where(
                e =>
                {
                    var lowerPath = e.ToLower();
                    return !lowerPath.EndsWith(".meta")
                           && (lowerPath.EndsWith(".prefab")
                               || lowerPath.EndsWith(".psd")
                               || lowerPath.EndsWith(".tiff")
                               || lowerPath.EndsWith(".jpg")
                               || lowerPath.EndsWith(".tga")
                               || lowerPath.EndsWith(".png")
                               || lowerPath.EndsWith(".bmp")
                               || lowerPath.EndsWith(".gif")
                               || lowerPath.EndsWith(".iff")
                               || lowerPath.EndsWith(".pict")
                               || lowerPath.EndsWith(".aif")
                               || lowerPath.EndsWith(".wav")
                               || lowerPath.EndsWith(".mp3")
                               || lowerPath.EndsWith(".ogg")
                               || lowerPath.EndsWith(".txt")
                               || lowerPath.EndsWith(".html")
                               || lowerPath.EndsWith(".htm")
                               || lowerPath.EndsWith(".xml")
                               || lowerPath.EndsWith(".bytes")
                               || lowerPath.EndsWith(".json")
                               || lowerPath.EndsWith(".csv")
                               || lowerPath.EndsWith(".yaml")
                               || lowerPath.EndsWith(".fnt")
                               || lowerPath.EndsWith(".ttf")
                               || lowerPath.EndsWith(".otf")
                               || lowerPath.EndsWith(".mat")
                               || lowerPath.EndsWith(".shader"));
                }).ToList();
            Debug.Log(prefabs.JoinToString("\n"));
            BundleExporterWindow.ShowWindow(prefabs);
        }

        private static List<string> ExtractObjectsPathFromFolder(DefaultAsset folderAsset)
        {
            var folderPath = AssetDatabase.GetAssetPath(folderAsset);
            folderPath = (string) folderPath.Split('/').SkipWhile(e => e != "Assets").Skip(1).JoinToString("/");
            var resourcesPaths = Directory.GetFiles(Application.dataPath +"/"+ folderPath, "*.*", SearchOption.AllDirectories);
            var relativePaths = resourcesPaths.Select(path => path.Split('/').SkipWhile(e => e != "Assets").JoinToString("/")).Select(e => e.EndsWith("/") ? e.Substring(0, e.Length - 1) : e);
            return relativePaths.ToList();
        }
    }
}
