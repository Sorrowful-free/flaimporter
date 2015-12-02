﻿using System;
using System.Collections;
using System.Linq;
using Assets.FlaExporter.Editor.Data.RawData;
using Assets.FlaExporter.Editor.EditorCoroutine;
using Assets.FlaExporter.Editor.Utils;
using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder;
using Assets.FlaExporter.FlaExporter.Renderer;
using Assets.FlaExporter.FlaExporter.Transorm;
using UnityEditor;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace Assets.FlaExporter.Editor.FlaProcessors
{
    public static class FlaProcessor
    {

        private static FlaDocumentRaw _currentFlaDocumentRaw;
        private static GameObject _currentRoot;
        public static IEnumerator ProcessFlaDocument(FlaDocumentRaw flaDocumentData)
        {
            _currentFlaDocumentRaw = flaDocumentData;
            var name = "FlaDocument" + flaDocumentData.GetHashCode();
            var documentGO = new GameObject(name);
            var colorAndFilters = documentGO.AddComponent<FlaColorAndFiltersHolder>();
            _currentRoot = documentGO;
            

            foreach (var timeline in flaDocumentData.Timelines)
            {
                yield return ProcessFlaTimeLineElements(timeline, elementGO =>
                {
                    elementGO.transform.SetParent(documentGO.transform,false);
                    var elementColorAndFilters = elementGO.GetComponent<FlaColorAndFiltersHolder>();
                    if (elementColorAndFilters != null)
                    {
                        colorAndFilters.AddChild(elementColorAndFilters);
                    }
                    var flaShape = elementGO.GetComponent<FlaShape>();
                    if (flaShape != null)
                    {
                        colorAndFilters.AddShape(flaShape);
                    }
                }).StartAsEditorCoroutine();
            }
            yield return null;


            foreach (var timeline in flaDocumentData.Timelines)
            {
                yield return
                   ProcessFlaTimeLine(timeline,
                       _currentFlaDocumentRaw == null ? 30 : _currentFlaDocumentRaw.FrameRate, _currentRoot).StartAsEditorCoroutine(); 
            }

            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.ExportedOutputFolder) + name + ".prefab", documentGO);
            yield return null;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            yield return null;
        }

        public static IEnumerator ProcessFlaSymbol(FlaSymbolItemRaw flaSymbolData)
        {
            var flaSymbolGO = new GameObject(flaSymbolData.Name);
            var colorAndFilters = flaSymbolGO.AddComponent<FlaColorAndFiltersHolder>();
            flaSymbolGO.AddComponent<FlaTransform>();

            _currentRoot = flaSymbolGO;
            
            yield return ProcessFlaTimeLineElements(flaSymbolData.Timeline.Timeline, elementGO =>
            {
                elementGO.transform.SetParent(flaSymbolGO.transform,false);
                var elementColorAndFilters = elementGO.GetComponent<FlaColorAndFiltersHolder>();
                if (elementColorAndFilters != null)
                {
                    colorAndFilters.AddChild(elementColorAndFilters);    
                }
                var flaShape = elementGO.GetComponent<FlaShape>();
                if (flaShape != null)
                {
                    colorAndFilters.AddShape(flaShape);
                }

            }).StartAsEditorCoroutine();
            yield return null;

            yield return
                ProcessFlaTimeLine(flaSymbolData.Timeline.Timeline,
                    _currentFlaDocumentRaw == null ? 30 : _currentFlaDocumentRaw.FrameRate, _currentRoot).StartAsEditorCoroutine();


            FolderAndFileUtils.CheckFolders(FoldersConstants.SymbolsFolder);
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.SymbolsFolder) + FolderAndFileUtils.RemoveUnacceptable(flaSymbolData.Name) + ".prefab", flaSymbolGO);
            GameObject.DestroyImmediate(flaSymbolGO);
            yield return null;
        }
        
        private static IEnumerator ProcessFlaTimeLineElements(FlaTimeLineRaw timeLine,Action<GameObject> callback)
        {
            foreach (var flaLayerRaw in timeLine.Layers)
            {
                var oredered = -(float)timeLine.Layers.IndexOf(flaLayerRaw);
                yield return FlaLayerProcessor.ProcessFlaLayerElement(flaLayerRaw, (go) =>
                {
                    if (callback != null)
                    {
                        var pos = go.transform.localPosition;
                        pos.z += oredered;
                        go.transform.localPosition = pos;

                        var scale = go.transform.localScale;
                        scale.z = 1 / (float)flaLayerRaw.Frames.SelectMany(e=>e.Elements).ToList().Count * 0.8f;
                        go.transform.localScale = scale;

                        callback(go);
                    }
                }).StartAsEditorCoroutine();
            }
            yield return null;
        }

        private static IEnumerator ProcessFlaTimeLine(FlaTimeLineRaw timeLine, int frameRate, GameObject root)
        {
            var frames = timeLine.Layers.SelectMany(e => e.Frames);
            if (frames.Max(e => e.Index) <= 0 && frames.Max(e => e.Duration) <= 0)
            {
                yield break;
            }
            FolderAndFileUtils.CheckFolders(FoldersConstants.AnimatorControllerFolder);
            var animationController = AnimatorController.CreateAnimatorControllerAtPath(FolderAndFileUtils.GetAssetFolder(FoldersConstants.AnimatorControllerFolder) + FolderAndFileUtils.RemoveUnacceptable(root.name) + "_AC.controller");
            root.AddComponent<Animator>().runtimeAnimatorController = animationController;
            FolderAndFileUtils.CheckFolders(FoldersConstants.AnimationClipsFolder);
            var animationClip = new AnimationClip() { name = "clip" };
            AssetDatabase.CreateAsset(animationClip,FolderAndFileUtils.GetAssetFolder(FoldersConstants.AnimationClipsFolder) + FolderAndFileUtils.RemoveUnacceptable(animationController.name) + ".anim");

            animationController.AddMotion(animationClip);

            foreach (var flaLayerRaw in timeLine.Layers)
            {
                yield return FlaLayerProcessor.ProcessFlaLayer(flaLayerRaw, frameRate, animationClip).StartAsEditorCoroutine();
            }
            yield return null;
        }

    }
}
