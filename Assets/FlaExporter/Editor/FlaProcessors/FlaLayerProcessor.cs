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
        public static IEnumerator ProcessFlaLayerElement(FlaLayerRaw layerData, Action<GameObject> callback)
        {
            if (!layerData.Visible)
            {
                yield break;
            }

            var layerGO = new GameObject(layerData.Name+layerData.GetHashCode());
            var frames = layerData.Frames;
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
           
        }

        public static IEnumerator ProcessFlaLayer(FlaLayerRaw layerData,int frameRate,AnimationClip clip)
        {
            //FolderAndFileUtils.CheckFolders(FoldersConstants.AnimationClipsFolder);
            Debug.Log("layerData type :"+layerData.LayerType);
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
        }

        private static IEnumerator ProcessDefaultFlaLayer(FlaLayerRaw layerData, int frameRate, AnimationClip clip)
        {
            if (!layerData.Visible)
            {
                yield break;
            }   
            Debug.LogWarning("================layer :"+layerData.Name);
            var allElementsInLayerName = layerData.Frames.SelectMany(e => e.Elements).Select(e=>e.GetName()).Distinct();
            var curves = new Dictionary<string, Dictionary<string,AnimationCurve>>();
            foreach (var frameRaw in layerData.Frames)
            {

                foreach (var elementName in allElementsInLayerName)
                {
                    var elementPath = layerData.Name + layerData.GetHashCode() + "/" + elementName;
                    var elementRaw = frameRaw.Elements.FirstOrDefault(e => e.GetName() == elementName);
                    var curveDictionary = default(Dictionary<string, AnimationCurve>);
                    if (!curves.TryGetValue(elementPath, out curveDictionary))
                    {
                        curveDictionary = new Dictionary<string, AnimationCurve>();
                        curves.Add(elementPath,curveDictionary);
                    }

                    var isVisible = elementRaw != null;
                    var visibleCurve = default(AnimationCurve);
                    if (!curveDictionary.TryGetValue("m_IsActive", out visibleCurve))
                    {
                        visibleCurve = new AnimationCurve();
                        curveDictionary.Add("m_IsActive", visibleCurve);
                    }

                    var currentVisible = isVisible ? 1.0f : 0.0f;
                    var lastVisible = visibleCurve.keys.Length>0?visibleCurve.keys[visibleCurve.keys.Length - 1].value:1;
                    if (visibleCurve.keys.Length > 1 && lastVisible == currentVisible)
                    {
                        visibleCurve.RemoveKey(visibleCurve.keys.Length-1);
                    }

                    if (visibleCurve.keys.Length <= 0 || currentVisible != lastVisible)
                    {
                        visibleCurve.AddKey((float)frameRaw.Index / (float)frameRate, currentVisible);
                    }
                    visibleCurve.AddKey(((float)frameRaw.Index + (float)Mathf.Max(1.0f,frameRaw.Duration)*0.99f) / (float)frameRate, currentVisible);
                    
                    if (elementRaw != null)
                    {
                        foreach (var key in FlaTransform.ProperyNames.Keys)
                        {
                            var curve = default(AnimationCurve);
                            if (!curveDictionary.TryGetValue(FlaTransform.ProperyNames[key], out curve))
                            {
                                curve = new AnimationCurve();
                                curveDictionary.Add(FlaTransform.ProperyNames[key], curve);
                            }
                            var lastKey = curve.keys.LastOrDefault();
                            var currentFrameValue = elementRaw.GetValueByPropertyType(key);
                            if ((curve.keys.Length <= 0 && currentFrameValue != FlaTransform.ProperyDefaultValues[key]) || lastKey.value != currentFrameValue)
                            {
                                curve.AddKey((float)frameRaw.Index / (float)frameRate, currentFrameValue);
                            }
                        }
                    }
                    yield return null;
                }
            }
            foreach (var curveDictionary in curves)
            {
                foreach (var curve in curveDictionary.Value)
                {
                    if (curve.Value.keys.Length > 0)
                    {
                        if (curve.Key == "m_IsActive")
                        {
                            curve.Value.SetCurveLinear();
                            clip.SetCurve(curveDictionary.Key, typeof(GameObject), curve.Key, curve.Value);
                        }
                        else
                        {
                            clip.SetCurve(curveDictionary.Key, typeof(FlaTransform), curve.Key, curve.Value);    
                        }
                    }
                }
            }
            Debug.LogWarning(layerData.Name + "end============");
            yield return null;
        }

        

    }
}
