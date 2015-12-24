﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.EditorCoroutine;
using Assets.FlaImporter.Editor.Extentions;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder.ColorTransform;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder.Enums;
using Assets.FlaImporter.FlaImporter.Transorm;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaLayerProcessor
    {
        public static IEnumerator ProcessFlaLayerElement(FlaLayerRaw layerData, Action<GameObject> callback)
        {
            if (!layerData.Visible || layerData.Frames == null || layerData.Frames.Count <= 0 || !layerData.Frames.SelectMany(e=>e.Elements).Any())
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
                    var order = (float)elements.IndexOf(element) / (float)elements.Count;
                  
                    var pos = elementGO.transform.localPosition;
                    pos.z = order;
                    elementGO.transform.localPosition = pos;

                    var scale = elementGO.transform.localScale;
                    scale.z = 1 / (float)elements.Count;
                    elementGO.transform.localScale = scale;

                    elementGO.name = layerData.Name + "_" + elementGO.name;
                    if (callback != null)
                    {
                        callback(elementGO);
                    }

                }).StartAsEditorCoroutine();
            }

            foreach (var instanceName in instancesNames)
            {
                var element = instances.FirstOrDefault(e => e.LibraryItemName == instanceName);
                yield return FlaFrameElementProcessor.ProcessFlaElement(element, (elementGO) =>
                {
                    var order = (float)elements.IndexOf(element) / (float)elements.Count;
                    var pos = elementGO.transform.localPosition;
                    
                    pos.z = order;
                    elementGO.transform.localPosition = pos;
                    
                    var scale = elementGO.transform.localScale ;
                    scale.z = 1/(float) elements.Count;
                    elementGO.transform.localScale = scale;

                    elementGO.name = layerData.Name +"_"+ elementGO.name;
                    if (callback != null)
                    {
                        callback(elementGO);
                    }

                }).StartAsEditorCoroutine();
            }

            yield return null;
        }

        public static IEnumerator ProcessFlaLayer(FlaLayerRaw layerData,int frameRate,AnimationClip clip)
        {
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

                case "folder": // folder ?
                    break;

                default: // classic animation or other
                    yield return ProcessDefaultFlaLayer(layerData, frameRate, clip).StartAsEditorCoroutine();
                    break;
            }
        }

        private static void WriteDuratibleKeys(float currentValue,AnimationCurve curve,FlaFrameRaw frameRaw,float frameRate)
        {
           // var currentVisible = isVisible ? 1.0f : 0.0f;
            var lastVisible = curve.keys.Length > 0 ? curve.keys[curve.keys.Length - 1].value : 1;
            if (curve.keys.Length > 1 && lastVisible == currentValue)
            {
                curve.RemoveKey(curve.keys.Length - 1);
            }

            if (curve.keys.Length <= 0 || currentValue != lastVisible)
            {
                curve.AddKey((float)frameRaw.Index / (float)frameRate, currentValue);
            }
            curve.AddKey(((float)frameRaw.Index + (float)Mathf.Max(1.0f, frameRaw.Duration)) / (float)frameRate - 1.0f / frameRate, currentValue);
        }
        private static IEnumerator ProcessDefaultFlaLayer(FlaLayerRaw layerData, int frameRate, AnimationClip clip)
        {
            if (!layerData.Visible)
            {
                yield break;
            }
            if (layerData.Frames.Max(e => e.Index) <= 0 && layerData.Frames.Max(e => e.Duration) <= 0)
            {
                yield break;
            }
            var allElementsInLayerName = layerData.Frames.SelectMany(e => e.Elements).Select(e=>e.GetName()).Distinct();
            var curvesTransform = new Dictionary<string, Dictionary<string,AnimationCurve>>();

            foreach (var frameRaw in layerData.Frames)
            {
                foreach (var elementName in allElementsInLayerName)
                {
                    var elementPath = layerData.Name+"_"+ elementName;
                    var elementRaw = frameRaw.Elements.FirstOrDefault(e => e.GetName() == elementName);
                    var curveTransformDictionary = default(Dictionary<string, AnimationCurve>);
                    if (!curvesTransform.TryGetValue(elementPath, out curveTransformDictionary))
                    {
                        curveTransformDictionary = new Dictionary<string, AnimationCurve>();
                        curvesTransform.Add(elementPath,curveTransformDictionary);
                    }

                    var isVisible = elementRaw != null;
                    var visibleCurve = default(AnimationCurve);
                    if (!curveTransformDictionary.TryGetValue("m_IsActive", out visibleCurve))
                    {
                        visibleCurve = new AnimationCurve();
                        curveTransformDictionary.Add("m_IsActive", visibleCurve);
                    }

                    var currentVisible = isVisible ? 1.0f : 0.0f;
                    WriteDuratibleKeys(currentVisible, visibleCurve, frameRaw, frameRate);
                    //var lastVisible = visibleCurve.keys.Length>0?visibleCurve.keys[visibleCurve.keys.Length - 1].value:1;
                    //if (visibleCurve.keys.Length > 1 && lastVisible == currentVisible)
                    //{
                    //    visibleCurve.RemoveKey(visibleCurve.keys.Length-1);
                    //}

                    //if (visibleCurve.keys.Length <= 0 || currentVisible != lastVisible)
                    //{
                    //    visibleCurve.AddKey((float)frameRaw.Index / (float)frameRate, currentVisible);
                    //}
                    //visibleCurve.AddKey(((float)frameRaw.Index + (float)Mathf.Max(1.0f,frameRaw.Duration)-0.01f) / (float)frameRate, currentVisible);
                    
                    if (elementRaw != null)
                    {
                        foreach (var key in FlaTransform.PropertyNames.Keys)
                        {
                            var curve = default(AnimationCurve);
                            if (!curveTransformDictionary.TryGetValue(FlaTransform.PropertyNames[key], out curve))
                            {
                                curve = new AnimationCurve();
                                curveTransformDictionary.Add(FlaTransform.PropertyNames[key], curve);
                            }
                            var lastKey = curve.keys.LastOrDefault();
                            var currentFrameValue = elementRaw.GetTransformValueByPropertyType(key);

                            if (frameRaw.Duration > 0 && frameRaw.TweenType != "motion")
                            {
                                WriteDuratibleKeys(currentFrameValue, curve, frameRaw, frameRate);
                            }
                            else
                            {
                                if (curve.keys.Length <= 0 || currentFrameValue != FlaTransform.ProperyDefaultValues[key] || lastKey.value != currentFrameValue)
                                {
                                    curve.AddKey((float)frameRaw.Index / (float)frameRate, currentFrameValue);
                                }    
                            }
                            
                        }
                        
                        foreach (var key in FlaColorAndFiltersHolder.PropertyNames.Keys)
                        {
                            switch (key)
                            {
                                case FlaColorAndFiltersHolderPropertyTypeEnum.SelfColorTransform:

                                    foreach (var subKey in FlaColorTransform.PropertyNames.Keys)
                                    {
                                        var curve = default(AnimationCurve);
                                        var curveName = FlaColorAndFiltersHolder.PropertyNames[key] + "." +
                                                        FlaColorTransform.PropertyNames[subKey];
                                        if (!curveTransformDictionary.TryGetValue(curveName, out curve))
                                        {
                                            curve = new AnimationCurve();
                                            curveTransformDictionary.Add(curveName, curve);
                                        }
                                        var currentFrameValue = elementRaw.GetColorValueByPropertyType(subKey);
                                        var lastKey = curve.keys.LastOrDefault();
                                        if (frameRaw.Duration > 0 && frameRaw.TweenType != "motion" )
                                        {
                                            WriteDuratibleKeys(currentFrameValue, curve, frameRaw, frameRate);
                                        }
                                        else
                                        {
                                            if (curve.keys.Length <= 0 || lastKey.value != currentFrameValue)
                                            {
                                                curve.AddKey((float) frameRaw.Index/(float) frameRate, currentFrameValue);
                                            }
                                        }
                                    }
                                    
                                    break;
                                default:
                                    break;
                            }
                           
                        }
                    }
                    yield return null;
                }
            }
            foreach (var curveDictionary in curvesTransform)
            {
                foreach (var curve in curveDictionary.Value)
                {
                    if (curve.Value.keys.Length > 0)
                    {
                        if (curve.Key == "m_IsActive")
                        {
                            curve.Value.SetCurveLinear();
                            clip.SetCurve(curveDictionary.Key, typeof (GameObject), curve.Key, curve.Value);
                        }
                        else if (curve.Key.Contains("_selfColorTransform"))
                        {
                            curve.Value.SetCurveLinear();
                            clip.SetCurve(curveDictionary.Key, typeof(FlaColorAndFiltersHolder), curve.Key, curve.Value);
                        }
                        else
                        {
                            curve.Value.SetCurveLinear();
                            clip.SetCurve(curveDictionary.Key, typeof(FlaTransform), curve.Key, curve.Value);
                        }
                        
                    }
                }
            }
            yield return null;
        }
    }
}