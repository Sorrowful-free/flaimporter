using System.Linq;
using Assets.FlaExporter.Data.RawData;
using Assets.FlaExporter.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.Editor
{
    public static class FlaProcessor
    {
        private const string BitmapSymbolsTextureFolderFolder = "/BitmapSymbols/Textures/";
        private const string BitmapSymbolsFolder = "/BitmapSymbols/";
        public static void ProcessFlaDocument(FlaDocumentRaw flaData)
        {
            //Debug.Log(flaData.PrettyPrintObjects());
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

        public  static void ProcessFlaSymbol(FlaSymbolItemRaw flaData)
        {
           
        }

        public static void ProcessFlaBitmapSymbol(FlaBitmapItemRaw flaBitmapItem)
        {
            FolderAndFileUtils.CheckFolders(BitmapSymbolsFolder);
            var bitmapSymbolGO = new GameObject(flaBitmapItem.Name);
            var bitmapSriteRenderer = bitmapSymbolGO.AddComponent<SpriteRenderer>();
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + BitmapSymbolsTextureFolderFolder + flaBitmapItem.Href);
            var spritesAsObjects = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(texture));
            var sprite = spritesAsObjects.FirstOrDefault(e => e.name == FolderAndFileUtils.RemoveExtention(flaBitmapItem.Href)) as Sprite;
            bitmapSriteRenderer.sprite = sprite;
            PrefabUtility.CreatePrefab("Assets"+BitmapSymbolsFolder + flaBitmapItem.Name+".prefab", bitmapSymbolGO);

        }



        
    }
}
