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
                yield return ProcessFlaTimeLine(timeline, elementGO =>
                {
                    elementGO.transform.SetParent(documentGO.transform);
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

            foreach (var flaLayerRaw in timeLine.Layers)
            {
                yield return ProcessFlaLayer(flaLayerRaw, callback).StartAsEditorCoroutine();
            }
            yield return null;

        }

        private static IEnumerator ProcessFlaLayer(FlaLayerRaw layer,Action<GameObject> callback)
        {
            var layerGO = new GameObject(layer.Name);

            var frames = layer.Frames;
            var elements = frames.SelectMany(e => e.Elements);
            var instances = elements.OfType<FlaBaseInstanceRaw>();
            var shapes = elements.OfType<FlaShapeRaw>();
            var instancesNames = instances.Select(e => e.LibraryItemName).Distinct();
            var shapesNames = shapes.Select(e => e.GetUniqueName()).Distinct();

            foreach (var shapesName in shapesNames)
            {
                var element = shapes.FirstOrDefault(e => e.GetUniqueName() == shapesName);
                yield return ProcessFlaElement(element, (elementGO) =>
                {
                    //if (callback != null)
                    //{
                    //    callback(elementGO);
                    //}
                    elementGO.transform.SetParent(layerGO.transform);
                }).StartAsEditorCoroutine();
            }

            foreach (var instanceName in instancesNames)
            {
                var element = instances.FirstOrDefault(e => e.LibraryItemName == instanceName);
                yield return ProcessFlaElement(element, (elementGO) =>
                {
                    //if (callback != null)
                    //{
                    //    callback(elementGO);
                    //}
                    elementGO.transform.SetParent(layerGO.transform);
                }).StartAsEditorCoroutine();

            }
            yield return null;
            
            if (callback != null)
            {
                callback(layerGO);
            }

            yield break;

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
                yield return ProcessFlaElement(element, (elementGO) =>
                {
                    elementGO.transform.SetParent(frameGO.transform);    
                }).StartAsEditorCoroutine();
            }
            if (callback != null)
            {
                callback(frameGO);
            }
            yield return null;
        }

        private static IEnumerator ProcessFlaElement(FlaFrameElementRaw element,Action<GameObject> callback)
        {
            var elementGO = default(GameObject);
            var shape = element as FlaShapeRaw;
            var instance = element as FlaBaseInstanceRaw;
            if (shape != null)
            {
                yield return ProcessFlaShape(shape, callback).StartAsEditorCoroutine();
                yield break;
            }

            if (instance != null)
            {
                yield return ProcessFlaInstance(instance, callback).StartAsEditorCoroutine();
            }
        }

        private static IEnumerator ProcessFlaInstance(FlaBaseInstanceRaw instance,Action<GameObject> callback)
        {
            var symbolInstance = instance as FlaSymbolInstanceRaw;
            var bitmapInstance = instance as FlaBitmapInstanceRaw;
            if (symbolInstance != null)
            {
                var symbol = ProcessFlaSymbolInstance(symbolInstance);
                if (callback != null)
                {
                    callback(symbol);
                }
                yield break;
            }
            if (bitmapInstance != null)
            {
                var bitmap = ProcessFlaBitmapInstance(bitmapInstance);
                if (callback != null)
                {
                    callback(bitmap);
                }
            }
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
            symbolGO.name = instance.LibraryItemName;
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
