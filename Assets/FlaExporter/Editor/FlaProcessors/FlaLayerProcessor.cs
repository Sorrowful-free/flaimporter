using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaExporter.Data.RawData;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.EditorCoroutine;
using Assets.FlaExporter.Editor.Extentions;
using Assets.FlaExporter.Editor.Utils;
using Assets.FlaExporter.FlaExporter;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.FlaExporter.Editor.FlaProcessors
{
    public static class FlaLayerProcessor
    {
        public static IEnumerator ProcessFlaLayerElement(FlaLayerRaw layer, Action<GameObject> callback)
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
                yield return FlaFrameElementProcessor.ProcessFlaElement(element, (elementGO) =>
                {
                    elementGO.transform.SetParent(layerGO.transform);
                    elementGO.AddComponent<FlaTransform>();

                }).StartAsEditorCoroutine();
            }

            foreach (var instanceName in instancesNames)
            {
                var element = instances.FirstOrDefault(e => e.LibraryItemName == instanceName);
                yield return FlaFrameElementProcessor.ProcessFlaElement(element, (elementGO) =>
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
            yield break;
            var animationPositionCurvesX = new Dictionary<string, AnimationCurve>();
            var animationPositionCurvesY = new Dictionary<string, AnimationCurve>();
            
            foreach (var frame in frames)
            {
                foreach (var elementRaw in frame.Elements)
                {
                    var position = elementRaw.Matrix.Matrix.GetPosition();
                    var path = layerGO.name + "/";
                    if (elementRaw is FlaShapeRaw)
                    {
                        var shape = elementRaw as FlaShapeRaw;
                        path += shape.GetUniqueName();
                    }
                    else if (elementRaw is FlaBaseInstanceRaw)
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
               //     curveX.AddKey((float)frame.Index / (float)frameRate, position.x);

                    var curveY = default(AnimationCurve);
                    if (!animationPositionCurvesY.TryGetValue(path, out curveY))
                    {
                        curveY = new AnimationCurve();
                        animationPositionCurvesY.Add(path, curveY);
                    }
                 //   curveY.AddKey((float)frame.Index / (float)frameRate, position.y);
                }
            }

            //clip.name = layerGO.name;
            //foreach (var key in animationPositionCurvesX.Keys)
            //{
            //    clip.SetCurve(key, typeof(Transform), "localPosition.x", animationPositionCurvesX[key]);
            //    clip.SetCurve(key, typeof(Transform), "localPosition.y", animationPositionCurvesY[key]);
            //}
        }

        public static IEnumerator ProcessFlaLayer(FlaLayerRaw layerData,int frameRate,AnimationClip clip)
        {
            //FolderAndFileUtils.CheckFolders(FoldersConstants.AnimationClipsFolder);
            Debug.Log("layer type :"+layerData.LayerType);
            switch (layerData.LayerType)
            {
                case "mask":
                    break;

                case "IK Pose":
                    break;

                case "motion object":
                    break;

                case "guide": // guid
                    break;

                case "folder": // guid
                    break;

                default: // classic animation or other
                    yield return ProcessDefaultFlaLayer(layerData, frameRate, clip).StartAsEditorCoroutine();
                    break;
            }
            yield break;
        }

        private static IEnumerator ProcessDefaultFlaLayer(FlaLayerRaw layerData, int frameRate, AnimationClip clip)
        {
            var curves = new Dictionary<string, Dictionary<string,AnimationCurve>>();
            foreach (var frameRaw in layerData.Frames)
            {
                foreach (var elementRaw in frameRaw.Elements)
                {
                    var elementPath = layerData.Name + "/" + elementRaw.GetName();
                    var curveDictionary = default(Dictionary<string, AnimationCurve>);
                    if (!curves.TryGetValue(elementPath, out curveDictionary))
                    {
                        curveDictionary = new Dictionary<string, AnimationCurve>();
                        curves.Add(elementPath,curveDictionary);
                    }

                    foreach (var key in FlaTransform.ProperyNames.Keys)
                    {
                        var curve = default(AnimationCurve);
                        if (!curveDictionary.TryGetValue(FlaTransform.ProperyNames[key], out curve))
                        {
                            curve = new AnimationCurve();
                            curveDictionary.Add(FlaTransform.ProperyNames[key], curve);
                        }
                        curve.AddKey((float) frameRaw.Index/(float) frameRate, elementRaw.GetValueByPropertyType(key));
                    }
                    yield return null;
                }

            }
            foreach (var curveDictionary in curves)
            {
                foreach (var curve in curveDictionary.Value)
                {
                    clip.SetCurve(curveDictionary.Key,typeof(FlaTransform),curve.Key,curve.Value);
                }
            }
            
            yield return null;
        }

        

    }
}
