using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.Editor.Utils;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder;
using Assets.FlaImporter.FlaImporter.Renderer;
using Assets.FlaImporter.FlaImporter.Transorm;
using Assets.FlaImporter.FlaImporter.Transorm.Enums;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaTimeLineProcessor
    {

        private static int GetLayerIndex(List<FlaLayerRaw> layers, FlaFrameElementRaw element)
        {
            var index = -1;
            var layer = layers.FirstOrDefault(l => l.Frames.SelectMany(f => f.Elements).Any(e => e.GetHashCode() == element.GetHashCode()));
            index = layers.IndexOf(layer);
            return index;
        }

        public static IEnumerator ProcessFlaTimeLine(FlaTimeLineRaw flaTimeLine, GameObject rootGO,int frameRate)
        {
            FlaObjectManager.ClearAll();
            FlaAnimationRecorder.Clear();

            #region instances Parse
            var lastFrameIndex = flaTimeLine.Layers.Max(l => l.Frames.Max(f => f.Index + f.Duration));
            var rootColorAndFilterHolder = rootGO.GetComponent<FlaColorAndFiltersHolder>();
            var animationClip = default(AnimationClip);
            if (lastFrameIndex > 1)
            {
                var animationController = AssetDataBaseUtility.CreateAnimatorController(rootGO.name);
                rootGO.AddComponent<Animator>().runtimeAnimatorController = animationController;
                animationClip = new AnimationClip() { name = rootGO.name + "_clip" };
                AssetDataBaseUtility.SaveAnimationClip(animationClip);

                animationController.AddMotion(animationClip);
            }
            
            foreach (var layerRaw in flaTimeLine.Layers)
            {
                
                var lastFrames = default(FlaFrameRaw);
                var layerIndex = flaTimeLine.Layers.IndexOf(layerRaw);
                foreach (var frameRaw in layerRaw.Frames)
                {
                    var time = (float) frameRaw.Index/(float) frameRate;

                    //if (_lastFrame != null)
                    //{
                    //    FlaAnimationRecorder.ReleaseFrameElements(,_lastFrame, frameRate);
                    //    foreach (var frameRaw in _lastFrame)
                    //    {
                            
                    //    }
                    //}
                    foreach (var elementRaw in frameRaw.Elements)
                    {
                        var elementGO = FlaObjectManager.GetFreeObject(elementRaw, layerIndex, (instance) =>
                        {
#region InstanceObjects 
                            var transform = instance.GetComponent<FlaTransform>();
                            transform.TransformPoint =
                                new Vector2(
                                    elementRaw.GetTransformValueByPropertyType(
                                        FlaTransformPropertyTypeEnum.TransformPointX),
                                    elementRaw.GetTransformValueByPropertyType(
                                        FlaTransformPropertyTypeEnum.TransformPointY));

                            transform.Rotation = elementRaw.Matrix.Matrix.GetAngle();
                            transform.Position = elementRaw.Matrix.Matrix.GetPosition();
                            transform.Scale = elementRaw.Matrix.Matrix.GetScale();
                            transform.Skew = elementRaw.Matrix.Matrix.GetSkew();
                            transform.TransformPoint = elementRaw.GetTransformPoint();

                            var color = instance.GetComponent<FlaColorAndFiltersHolder>();
                            if (color != null) // only on the shape can be null
                            {
                                var colorTransform = color.SelfColorTransform;
                                colorTransform.ColorMultipler = elementRaw.Color.Color.GetColorMultipler();
                                colorTransform.ColorOffset = elementRaw.Color.Color.GetColorOffset();
                                color.SelfColorTransform = colorTransform;
                            }

                            var shape = instance.GetComponent<FlaShape>();
                            var colorAndFilterHolder = instance.GetComponent<FlaColorAndFiltersHolder>();
                            if (shape != null)
                            {
                                rootColorAndFilterHolder.AddShape(shape);
                            }
                            else if (colorAndFilterHolder != null)
                            {
                                rootColorAndFilterHolder.AddChild(colorAndFilterHolder);

                              //  var layerRaw = flaTimeLine.Layers[GetLayerIndex(flaTimeLine.Layers, elementRaw)];//GetElementLayer(flaTimeLine, elementRaw);
                                if (layerRaw.ParentLayerIndex >= 0)
                                {
                                    var parentLayer = flaTimeLine.Layers[layerRaw.ParentLayerIndex];
                                    Debug.Log(instance.name + " parent index " + layerRaw.ParentLayerIndex + " e:" + frameRaw.GetHashCode() + " L:" + layerRaw.Name + "(" + layerRaw.GetHashCode() + ")");
                                    switch (parentLayer.LayerType)
                                    {
                                        case "mask":
                                            colorAndFilterHolder.MaskType = FlaColorAndFiltersHolderMaskType.Masked;
                                            colorAndFilterHolder.MaskId = layerRaw.ParentLayerIndex;
                                            break;
                                        case "guide":
                                            colorAndFilterHolder.GuidType = FlaColorAndFiltersHolderGuidType.Guided;
                                            colorAndFilterHolder.GuidId = layerRaw.ParentLayerIndex;
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (layerRaw.LayerType)
                                    {
                                        case "mask":
                                            colorAndFilterHolder.MaskType = FlaColorAndFiltersHolderMaskType.Mask;
                                            colorAndFilterHolder.MaskId = flaTimeLine.Layers.IndexOf(layerRaw);
                                            break;
                                        case "guide":
                                             colorAndFilterHolder.GuidType = FlaColorAndFiltersHolderGuidType.Guid;
                                             colorAndFilterHolder.GuidId = flaTimeLine.Layers.IndexOf(layerRaw);
                                            break;
                                    }
                                }

                            }

                            var position = instance.transform.position;
                            position.z = GetLayerIndex(flaTimeLine.Layers, elementRaw);
                            instance.transform.position = position;

                            instance.transform.SetParent(rootGO.transform, false);
#endregion
                        });
                        FlaAnimationRecorder.RecordFrameElement(elementGO.name, elementRaw, time);
                        
                    }
                    FlaObjectManager.ReleaseAll(layerIndex);
                }
                //layerGO.transform.SetParent(rootGO.transform,false);
                FlaAnimationRecorder.ApplyToClip(animationClip);
            }
            var scale = rootGO.transform.localScale;
            scale.z = 1.0f / (float)rootGO.transform.childCount;
            rootGO.transform.localScale = scale; 
            yield return 0;
            //for (int i = 0; i <= lastFrameIndex; i++) 
            //{
            //    FlaObjectManager.ReleaseAll();
            //    var elements = GetElementsInFrame(flaTimeLine, i);
            //    foreach (var elementRaw in elements)
            //    {
            //        //Debug.Log(elementRaw.GetName()+" parent index: " +GetElementLayer(flaTimeLine,elementRaw).ParentLayerIndex);
            //        var elementGO = FlaObjectManager.GetFreeObject(elementRaw, (instance) =>
            //        {
            //            var count = elements.Count(e => e.GetName() == elementRaw.GetName());
            //            if (count > 1)
            //            {
            //                Debug.Log("frame index " + i + " element count " + count);
            //                Debug.Log(elements.Where(e => e.GetName() == elementRaw.GetName()).Select(e => GetElementLayer(flaTimeLine, e).Name).JoinToString(", "));    
            //            }

            //            var transform = instance.GetComponent<FlaTransform>();
            //            transform.TransformPoint =
            //                new Vector2(
            //                    elementRaw.GetTransformValueByPropertyType(
            //                        FlaTransformPropertyTypeEnum.TransformPointX),
            //                    elementRaw.GetTransformValueByPropertyType(
            //                        FlaTransformPropertyTypeEnum.TransformPointY));

            //            transform.Rotation = elementRaw.Matrix.Matrix.GetAngle();
            //            transform.Position = elementRaw.Matrix.Matrix.GetPosition();
            //            transform.Scale = elementRaw.Matrix.Matrix.GetScale();
            //            transform.Skew = elementRaw.Matrix.Matrix.GetSkew();
            //            transform.TransformPoint = elementRaw.GetTransformPoint();

            //            var color = instance.GetComponent<FlaColorAndFiltersHolder>();
            //            if (color != null) // only on the shape can be null
            //            {
            //                var colorTransform = color.SelfColorTransform;
            //                colorTransform.ColorMultipler = elementRaw.Color.Color.GetColorMultipler();
            //                colorTransform.ColorOffset = elementRaw.Color.Color.GetColorOffset();
            //                color.SelfColorTransform = colorTransform;
            //            }

            //            var shape = instance.GetComponent<FlaShape>();
            //            var colorAndFilterHolder = instance.GetComponent<FlaColorAndFiltersHolder>();
            //            if (shape != null)
            //            {
            //                rootColorAndFilterHolder.AddShape(shape);
            //            }
            //            else if (colorAndFilterHolder != null)
            //            {
            //                rootColorAndFilterHolder.AddChild(colorAndFilterHolder);

            //                var layerRaw = flaTimeLine.Layers[GetLayerIndex(flaTimeLine.Layers, elementRaw)];//GetElementLayer(flaTimeLine, elementRaw);
            //                if (layerRaw.ParentLayerIndex >= 0)
            //                {
            //                    var parentLayer = flaTimeLine.Layers[layerRaw.ParentLayerIndex];
            //                    Debug.Log(instance.name + " parent index " + layerRaw.ParentLayerIndex + " e:" + elementRaw.GetHashCode() + " L:" + layerRaw.Name+"("+layerRaw.GetHashCode()+")");
            //                    switch (parentLayer.LayerType)
            //                    {
            //                        case "mask":
            //                            colorAndFilterHolder.MaskType = FlaColorAndFiltersHolderMaskType.Masked;
            //                            colorAndFilterHolder.MaskId = layerRaw.ParentLayerIndex;
            //                            break;
            //                        case "guide":
            //                            colorAndFilterHolder.MaskType = FlaColorAndFiltersHolderMaskType.Guided;
            //                            colorAndFilterHolder.MaskId = layerRaw.ParentLayerIndex;
            //                            break;
            //                    }
            //                }
            //                else
            //                {
            //                    switch (layerRaw.LayerType)
            //                    {
            //                        case "mask":
            //                            colorAndFilterHolder.MaskType = FlaColorAndFiltersHolderMaskType.Mask;
            //                            colorAndFilterHolder.MaskId = flaTimeLine.Layers.IndexOf(layerRaw);
            //                            break;
            //                        case "guide":
            //                            colorAndFilterHolder.MaskType = FlaColorAndFiltersHolderMaskType.Guid;
            //                            colorAndFilterHolder.MaskId = flaTimeLine.Layers.IndexOf(layerRaw);
            //                            break;
            //                    }
            //                }

            //            }

            //            var position = instance.transform.position;
            //            position.z = GetLayerIndex(flaTimeLine.Layers, elementRaw);
            //            instance.transform.position = position;

            //            instance.transform.SetParent(rootGO.transform, false);
            //        });
            //      //  yield return 0;
            //    }
            //    yield return 0;
            //}

            //var scale = rootGO.transform.localScale;
            //scale.z = 1.0f/(float) rootGO.transform.childCount;
            //rootGO.transform.localScale = scale;

            #endregion



            //if (lastFrameIndex > 1)
            //{
            //    #region apply anaimation
            //    //Debug.Log("--------------------------------------------------------------------------------------------------");
            //    //Debug.Log("-------------------------------------begin write animation----------------------------------------");
            //    //Debug.Log("--------------------------------------------------------------------------------------------------");
            //    var animationController = AssetDataBaseUtility.CreateAnimatorController(rootGO.name);
            //    rootGO.AddComponent<Animator>().runtimeAnimatorController = animationController;
            //    var animationClip = new AnimationClip() { name = rootGO.name + "_clip" };
            //    AssetDataBaseUtility.SaveAnimationClip(animationClip);

            //    animationController.AddMotion(animationClip);


            //    List<FlaFrameRaw> lastFrames = new List<FlaFrameRaw>();
            //    for (int i = 0; i <= lastFrameIndex; i++)
            //    {
            //        var temp = GetFramesByIndex(flaTimeLine, i);
            //        var needRecordedFrames = temp.Where(t => lastFrames.All(l => l != t)).ToList();
            //        var needRemoveFrames = lastFrames.Where(l => temp.All(t => t != l)).ToList();
            //   //     Debug.Log(string.Format("index:{0},\nnrec:{1},\nnrem:{2}", i, needRecordedFrames.JoinToString(", "), needRemoveFrames.JoinToString(", ")));

            //        foreach (var frameRaw in needRemoveFrames)
            //        {
            //            FlaAnimationRecorder.ReleaseFrameElements(frameRaw, frameRate);
            //        }

            //        foreach (var frameRaw in needRecordedFrames)
            //        {
            //            FlaAnimationRecorder.RecordFrameElements(frameRaw, frameRate);
            //        }

            //        lastFrames.AddRange(needRecordedFrames);
            //        lastFrames.RemoveAll(e => needRemoveFrames.Any(r => r == e));
            //    }
            //    FlaAnimationRecorder.ApplyToClip(animationClip);
            //    #endregion
            //}
            //else
            //{
            //    foreach (var layerRaw in flaTimeLine.Layers)
            //    {
            //        foreach (var frameRaw in layerRaw.Frames)
            //        {
            //            foreach (var elementRaw in frameRaw.Elements)
            //            {
            //                var elementGO = FlaObjectManager.GetFreeObject(elementRaw);
            //                var index = frameRaw.Elements.IndexOf(elementRaw);
            //                var transform = elementGO.GetComponent<FlaTransform>();
            //                transform.Order = -index;
            //            }
            //        }
            //    }
            //}

        }

        private static List<FlaFrameElementRaw> GetElementsInFrame(FlaTimeLineRaw flaTimeLine, int index)
        {
            var temp = GetFramesByIndex(flaTimeLine, index);
            var tempElements = temp.SelectMany(e => e.Elements).ToList();
            //Debug.Log(string.Format("frames count {0} elements Count {1}",temp.Count,tempElements.Count));
            return tempElements;
        }

        private static List<FlaFrameRaw> GetFramesByIndex(FlaTimeLineRaw flaTimeLine, int index)
        {
            return
                flaTimeLine.Layers.Where(e=>e.Visible).SelectMany(e => e.Frames)
                    .Where(e => index >= e.Index && index < (e.Index + e.Duration))
                    .ToList();
        }

        public static FlaLayerRaw GetElementLayer(FlaTimeLineRaw timeLineRaw, FlaFrameElementRaw elemenRaw)
        {
            return timeLineRaw.Layers.FirstOrDefault(l => l.Frames.SelectMany(f => f.Elements).Any(e => e == elemenRaw));
        }
    }
}
