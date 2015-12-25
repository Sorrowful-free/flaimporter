using System;
using System.Collections;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.EditorCoroutine;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.Editor.Utils;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder;
using Assets.FlaImporter.FlaImporter.Renderer;
using Assets.FlaImporter.FlaImporter.Transorm;
using UnityEditor;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace Assets.FlaImporter.Editor.FlaProcessors
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
                       _currentFlaDocumentRaw == null ? 30 : _currentFlaDocumentRaw.FrameRate, documentGO).StartAsEditorCoroutine();
                yield return 0; 
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


            AssetDataBaseUtility.SaveSymbol(flaSymbolGO);
            GameObject.DestroyImmediate(flaSymbolGO);
            yield return null;
        }

        public static IEnumerator ProcessFlaLayerElement(FlaLayerRaw layerData)
        {
            if (!layerData.Visible || layerData.Frames == null || layerData.Frames.Count <= 0 || !layerData.Frames.SelectMany(e => e.Elements).Any())
            {
                yield break;
            }

            var frames = layerData.Frames;
            var elements = frames.SelectMany(e => e.Elements).ToList();

            var instances = elements.OfType<FlaBaseInstanceRaw>();
            var instancesNames = instances.Select(e => e.LibraryItemName).Distinct();

            var shapes = elements.OfType<FlaShapeRaw>();
            var shapesNames = shapes.Select(e => e.GetUniqueName()).Distinct();


            foreach (var shapesName in shapesNames)
            {
                var element = shapes.FirstOrDefault(e => e.GetUniqueName() == shapesName);
                yield return FlaFrameElementProcessor.ProcessFlaElement(element, (elementGO) =>
                {
                    GameObject.DestroyImmediate(elementGO);
                }).StartAsEditorCoroutine();
            }

            foreach (var instanceName in instancesNames)
            {
                var element = instances.FirstOrDefault(e => e.LibraryItemName == instanceName);
                yield return FlaFrameElementProcessor.ProcessFlaElement(element, (elementGO) =>
                {
                    GameObject.DestroyImmediate(elementGO);
                }).StartAsEditorCoroutine();
            }

            yield return null;
        }

        private static IEnumerator ProcessFlaTimeLineElements(FlaTimeLineRaw timeLine,Action<GameObject> callback)
        {
            foreach (var flaLayerRaw in timeLine.Layers)
            {
                yield return ProcessFlaLayerElement(flaLayerRaw).StartAsEditorCoroutine();
            }
            yield return null;
        }

        private static IEnumerator ProcessFlaTimeLine(FlaTimeLineRaw timeLine, int frameRate, GameObject root)
        {

            yield return FlaTimeLineProcessor.ProcessFlaTimeLine(timeLine, root, frameRate).StartAsEditorCoroutine();
        }

    }
}
