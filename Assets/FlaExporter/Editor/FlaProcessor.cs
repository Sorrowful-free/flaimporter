using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaExporter.Data.RawData;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.Extentions;
using Assets.FlaExporter.Editor.Utils;
using Assets.FlaExporter.FlaExporter;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.FlaExporter.Editor
{
    public static class FlaProcessor
    {

        private static FlaDocumentRaw _currentFlaDocumentRaw;
        private static GameObject _currentFlaDocumentGO;
        private static AnimatorController _currentFlaDocumentAC;
        public static IEnumerator ProcessFlaDocument(FlaDocumentRaw flaDocumentData)
        {
            
            var name = "FlaDocument" + flaDocumentData.GetHashCode();
            var documentGO = new GameObject(name);
            var anim = documentGO.AddComponent<Animator>();
            _currentFlaDocumentGO = documentGO;

            FolderAndFileUtils.CheckFolders(FoldersConstants.AnimatorControllerFolder);
            _currentFlaDocumentAC = AnimatorController.CreateAnimatorControllerAtPath(FolderAndFileUtils.GetAssetFolder(FoldersConstants.AnimatorControllerFolder) + name+".controller");
            
            anim.runtimeAnimatorController = _currentFlaDocumentAC;


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
            _currentFlaDocumentAC.AddMotion(clip);
            _currentFlaDocumentGO = null;
            _currentFlaDocumentAC = null;
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

        private static AnimationClip clip = new AnimationClip();

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
                    elementGO.transform.SetParent(layerGO.transform);
                    elementGO.AddComponent<FlaTransform>();
                    
                }).StartAsEditorCoroutine();
            }

            foreach (var instanceName in instancesNames)
            {
                var element = instances.FirstOrDefault(e => e.LibraryItemName == instanceName);
                yield return ProcessFlaElement(element, (elementGO) =>
                {
                    elementGO.transform.SetParent(layerGO.transform);
                    elementGO.AddComponent<FlaTransform>();
                   
                }).StartAsEditorCoroutine();

            }
            yield return null;
            
            if (callback != null)
            {
                callback(layerGO);
            }

            
            var animationPositionCurvesX = new Dictionary<string,AnimationCurve>();
            var animationPositionCurvesY = new Dictionary<string, AnimationCurve>();
          

            foreach (var frame in frames)
            {
                foreach (var elementRaw in frame.Elements)
                {
                    //elementRaw.Matrix.Matrix.GetAngle()
                    var position = elementRaw.Matrix.Matrix.GetPosition();
                    var path = layerGO.name + "/";
                    if (elementRaw is FlaShapeRaw)
                    {
                        var shape = elementRaw as FlaShapeRaw;
                        path += shape.GetUniqueName();
                    }
                    else if(elementRaw is FlaBaseInstanceRaw)
                    {
                        var instance = elementRaw as FlaBaseInstanceRaw;
                        path += instance.LibraryItemName;
                    }
                    
                    var curveX = default(AnimationCurve);
                    if (!animationPositionCurvesX.TryGetValue(path, out curveX))
                    {
                        curveX = new AnimationCurve();
                        animationPositionCurvesX.Add(path, curveX);
                    }
                    curveX.AddKey((float)frame.Index / (float)(_currentFlaDocumentRaw == null ? 30 : _currentFlaDocumentRaw.FrameRate), position.x);

                    var curveY = default(AnimationCurve);
                    if (!animationPositionCurvesY.TryGetValue(path, out curveY))
                    {
                        curveY = new AnimationCurve();
                        animationPositionCurvesY.Add(path, curveY);
                    }
                    curveY.AddKey((float)frame.Index / (float)(_currentFlaDocumentRaw == null ? 30 : _currentFlaDocumentRaw.FrameRate), position.y);
                }
            }
            
            clip.name = layerGO.name;
            foreach (var key in animationPositionCurvesX.Keys)
            {
                clip.SetCurve(key, typeof(Transform), "localPosition.x", animationPositionCurvesX[key]);
                clip.SetCurve(key, typeof(Transform), "localPosition.y", animationPositionCurvesY[key]);    
            }

            


        }

        private static IEnumerator ProcessFlaFrame(FlaFrameRaw frame)
        {

            //Debug.Log(_currentFlaDocumentGO.transform.GetTransformPath());
            //frame.Elements
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
            _currentFlaDocumentGO = flaSymbolGO;
            var anim = flaSymbolGO.AddComponent<Animator>();
            _currentFlaDocumentGO = flaSymbolGO;
            FolderAndFileUtils.CheckFolders(FoldersConstants.AnimatorControllerFolder);
            _currentFlaDocumentAC = AnimatorController.CreateAnimatorControllerAtPath(FolderAndFileUtils.GetAssetFolder(FoldersConstants.AnimatorControllerFolder) + flaSymbolData.Name + ".controller");
            anim.runtimeAnimatorController = _currentFlaDocumentAC;

            yield return ProcessFlaTimeLine(flaSymbolData.Timeline.Timeline, timeLineGO =>
            {
                timeLineGO.transform.SetParent(flaSymbolGO.transform);
            }).StartAsEditorCoroutine();
            yield return null;

            FolderAndFileUtils.CheckFolders(FoldersConstants.SymbolsFolder);
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.SymbolsFolder) + flaSymbolData.Name + ".prefab", flaSymbolGO);
            GameObject.DestroyImmediate(flaSymbolGO);
            _currentFlaDocumentGO = null;
            _currentFlaDocumentAC = null;
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
