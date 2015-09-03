using System.Linq;
using Assets.FlaExporter.Data.RawData;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.Editor
{
    public static class FlaProcessor
    {
        
        public static void ProcessFlaDocument(FlaDocumentRaw flaDocumentData)
        {
            var name = "FlaDocument" + flaDocumentData.GetHashCode();
            var documentGO = new GameObject(name);
            foreach (var timeline in flaDocumentData.Timelines)
            {
                var timelineGO = ProcessFlaTimeLine(timeline);
                timelineGO.transform.SetParent(documentGO.transform); 
            }
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.ExportedOutputFolder) + name + ".prefab", documentGO);
        }

        
        public static void ProcessFlaSymbol(FlaSymbolItemRaw flaSymbolData, bool createInstance = false)
        {
            var flaSymbolGO = new GameObject(flaSymbolData.Name);
            var timeLineGO = ProcessFlaTimeLine(flaSymbolData.Timeline.Timeline);
            timeLineGO.transform.SetParent(flaSymbolGO.transform);
            FolderAndFileUtils.CheckFolders(FoldersConstants.SymbolsFolder);
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.SymbolsFolder) + flaSymbolData.Name + ".prefab", flaSymbolGO);
            if (!createInstance)
            {
                GameObject.DestroyImmediate(flaSymbolGO);
            }
        }

        private static GameObject ProcessFlaTimeLine(FlaTimeLineRaw timeLine)
        {
            var timeLineGO = new GameObject(timeLine.Name);
            foreach (var layer in timeLine.Layers)
            {
                var layerGO = ProcessFlaLayer(layer);
                layerGO.transform.SetParent(timeLineGO.transform);
            }
            return timeLineGO;
        }

        private static GameObject ProcessFlaLayer(FlaLayerRaw layer)
        {
            var layerGO = new GameObject(layer.Name);
            foreach (var frame in layer.Frames)
            {
                var frameGO = ProcessFlaFrame(frame);
                frameGO.transform.SetParent(layerGO.transform);
            }
            return layerGO;
        }

        private static GameObject ProcessFlaFrame(FlaFrameRaw frame)
        {
            var frameGO = new GameObject("frame"+frame.Index + (frame.Name == null ? "":frame.Name));
            foreach (var element in frame.Elements)
            {
                var elementGO = default(GameObject);
                var instance = element as FlaSymbolInstanceRaw;
                var shape = element as FlaShapeRaw;
                if (instance != null)
                {
                    elementGO = ProcessSymbolInstance(instance);
                }
                else if (shape != null)
                {
                    elementGO = ProcessFlaShape(shape);
                }
                
                elementGO.transform.SetParent(frameGO.transform);
            }
            return frameGO;
        }

        public static GameObject ProcessSymbolInstance(FlaSymbolInstanceRaw instance)
        {
            return GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(FolderAndFileUtils.GetAssetFolder(FoldersConstants.SymbolsFolder) + instance.LibraryItemName+".prefab"));
        }
        public static GameObject ProcessFlaShape(FlaShapeRaw shape)
        {
            return new GameObject("shape"+shape.GetHashCode());
        }
        
        public static void ProcessFlaBitmapSymbol(FlaBitmapItemRaw flaBitmapItem,bool createInstance = false)
        {
            FolderAndFileUtils.CheckFolders(FoldersConstants.BitmapSymbolsFolder);
            var bitmapSymbolGO = new GameObject(flaBitmapItem.Name);
            var bitmapSriteRenderer = bitmapSymbolGO.AddComponent<SpriteRenderer>();
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsTextureFolderFolder + flaBitmapItem.Href));
            var spritesAsObjects = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(texture));
            var sprite = spritesAsObjects.FirstOrDefault(e => e.name == FolderAndFileUtils.RemoveExtention(flaBitmapItem.Href)) as Sprite;
            bitmapSriteRenderer.sprite = sprite;
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsFolder) + flaBitmapItem.Name + ".prefab", bitmapSymbolGO);
            if (!createInstance)
            {
                GameObject.DestroyImmediate(bitmapSymbolGO);
            }
        }



        
    }
}
