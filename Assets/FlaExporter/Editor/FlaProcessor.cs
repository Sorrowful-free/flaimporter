using System;
using System.Collections;
using System.Linq;
using Assets.FlaExporter.Data.RawData;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.Extentions;
using Assets.FlaExporter.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.Editor
{
    public static class FlaProcessor
    {
        
        public static IEnumerator ProcessFlaDocument(FlaDocumentRaw flaDocumentData)
        {
            var name = "FlaDocument" + flaDocumentData.GetHashCode();
            var documentGO = new GameObject(name);

            foreach (var timeline in flaDocumentData.Timelines)
            {
                yield return ProcessFlaTimeLine(timeline, timelineGO =>
                {
                    timelineGO.transform.SetParent(documentGO.transform); 
                }).StartAsEditorCoroutine();
            }
            yield return null;
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.ExportedOutputFolder) + name + ".prefab", documentGO);
            yield return null;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            yield return null;
        }

       
        
        private static IEnumerator ProcessFlaTimeLine(FlaTimeLineRaw timeLine,Action<GameObject> callback)
        {
            var timeLineGO = new GameObject(timeLine.Name);
            foreach (var layer in timeLine.Layers)
            {
                yield return ProcessFlaLayer(layer, layerGO =>
                {
                    layerGO.transform.SetParent(timeLineGO.transform);
                }).StartAsEditorCoroutine();
               
            }
            yield return null;
            if (callback != null)
            {
                callback(timeLineGO);
            }
            yield return null;
        }

        private static IEnumerator ProcessFlaLayer(FlaLayerRaw layer,Action<GameObject> callback)
        {
            var layerGO = new GameObject(layer.Name);
            foreach (var frame in layer.Frames)
            {
                yield return ProcessFlaFrame(frame, frameGO =>
                {
                    frameGO.transform.SetParent(layerGO.transform);
                }).StartAsEditorCoroutine();
            }
            if (callback != null)
            {
                callback(layerGO);
            }
            yield return null;
        }

        private static IEnumerator ProcessFlaFrame(FlaFrameRaw frame,Action<GameObject> callback)
        {
            var frameGO = new GameObject("frame"+frame.Index + (frame.Name == null ? "":frame.Name));
            foreach (var element in frame.Elements)
            {
                var elementGO = default(GameObject);
                var instance = element as FlaSymbolInstanceRaw;
                var shape = element as FlaShapeRaw;
                var bitmap = element as FlaBitmapInstanceRaw;
                if (instance != null)
                {
                    elementGO = ProcessFlaSymbolInstance(instance);
                }
                else if (shape != null)
                {
                    yield return ProcessFlaShape(shape, o =>
                    {
                        elementGO = o;
                    }).StartAsEditorCoroutine();
                }
                else if (bitmap != null) 
                {
                    elementGO = ProcessFlaBitmapInstance(bitmap);
                }
                
                elementGO.transform.SetParent(frameGO.transform);
                yield return null;
            }
            if (callback != null)
            {
                callback(frameGO);
            }
            yield return null;
        }

        public static IEnumerator ProcessFlaSymbol(FlaSymbolItemRaw flaSymbolData)
        {
            var flaSymbolGO = new GameObject(flaSymbolData.Name);
            yield return ProcessFlaTimeLine(flaSymbolData.Timeline.Timeline, timeLineGO =>
            {
                timeLineGO.transform.SetParent(flaSymbolGO.transform);
            }).StartAsEditorCoroutine();
            yield return null;
           // timeLineGO.transform.SetParent(flaSymbolGO.transform);
            FolderAndFileUtils.CheckFolders(FoldersConstants.SymbolsFolder);
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.SymbolsFolder) + flaSymbolData.Name + ".prefab", flaSymbolGO);
            GameObject.DestroyImmediate(flaSymbolGO);
            yield return null;
        }
        
        public static GameObject ProcessFlaSymbolInstance(FlaSymbolInstanceRaw instance)
        {
            var symbolGO = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(FolderAndFileUtils.GetAssetFolder(FoldersConstants.SymbolsFolder) + instance.LibraryItemName+".prefab"));
            if (instance.Matrix != null && instance.Matrix.Matrix != null)
            {
                instance.Matrix.Matrix.CopyMatrix(symbolGO.transform);
            }
            return symbolGO;
        }

        public static GameObject ProcessFlaBitmapInstance(FlaBitmapInstanceRaw instance)
        {
            var bitmapSymbolGO = new GameObject(instance.LibraryItemName);
            var bitmapSriteRenderer = bitmapSymbolGO.AddComponent<SpriteRenderer>();
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsTextureFolderFolder) + instance.LibraryItemName);
            var spritesAsObjects = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(texture));
            var sprite = spritesAsObjects.FirstOrDefault(e => e.name == FolderAndFileUtils.RemoveExtention(instance.LibraryItemName)) as Sprite;
            bitmapSriteRenderer.sprite = sprite;

            instance.Matrix.Matrix.CopyMatrix(bitmapSymbolGO.transform);
            return bitmapSymbolGO;// GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsFolder) + instance.LibraryItemName + ".prefab"));
        }
        
        public static IEnumerator ProcessFlaShape(FlaShapeRaw shape,Action<GameObject> callback)
        {
            return FlaShapeProcessor.ProcessFlaShape(shape,callback);
        }

       

       
    }
}
