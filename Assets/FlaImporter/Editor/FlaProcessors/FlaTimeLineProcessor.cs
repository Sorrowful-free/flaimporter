using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.Extentions;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.Editor.Utils;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder;
using Assets.FlaImporter.FlaImporter.Renderer;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaTimeLineProcessor
    {
       

        public static IEnumerator ProcessFlaTimeLine(FlaTimeLineRaw flaTimeLine, GameObject rootGO,int frameRate)
        {
      //      Debug.LogWarning("start record animation");
            FlaObjectManager.Clear();
            FlaAnimationRecorder.Clear();
            var animationController = AssetDataBaseUtility.CreateAnimatorController(rootGO.name);
            rootGO.AddComponent<Animator>().runtimeAnimatorController = animationController;
            var animationClip = new AnimationClip() { name = rootGO.name+"_clip" };
            AssetDataBaseUtility.SaveAnimationClip(animationClip);

            animationController.AddMotion(animationClip);

            #region instances Parse
            var lastFrameIndex = flaTimeLine.Layers.Max(l => l.Frames.Max(f => f.Index + f.Duration));
            var rootColorAndFilterHolder = rootGO.GetComponent<FlaColorAndFiltersHolder>();
            for (int i = 0; i <= lastFrameIndex; i++)
            {
                FlaObjectManager.ReleaseAll();
                var elements = GetElementsInFrame(flaTimeLine, i);
                foreach (var elementRaw in elements)
                {
                    var elementGO = FlaObjectManager.GetFreeObject(elementRaw);
                    var shape = elementGO.GetComponent<FlaShape>();
                    var colorAndFilterHolder = elementGO.GetComponent<FlaColorAndFiltersHolder>();
                    if (shape != null)
                    {
                        rootColorAndFilterHolder.AddShape(shape);
                    }
                    else if (colorAndFilterHolder != null)
                    {
                        rootColorAndFilterHolder.AddChild(colorAndFilterHolder);
                    }
                    elementGO.transform.SetParent(rootGO.transform, false);
                    yield return 0;
                }
                yield return 0;
            }
            #endregion

            #region apply anaimation
            List<FlaFrameRaw> lastFrames = new List<FlaFrameRaw>();
            for (int i = 0; i <= lastFrameIndex; i++)
            {
                var temp = GetFramesByIndex(flaTimeLine, i);
                var needRecordedFrames = temp.Where(t => lastFrames.All(l => l != t)).ToList();
                var needRemoveFrames = lastFrames.Where(l => temp.All(t => t != l)).ToList();
                Debug.Log(string.Format("index:{0},\nnrec:{1},\nnrem:{2}",i,needRecordedFrames.JoinToString(", "),needRemoveFrames.JoinToString(", ")));

                Debug.Log("try remove" + needRemoveFrames.ToList().Count);
                foreach (var frameRaw in needRemoveFrames)
                {
                    FlaAnimationRecorder.ReleaseFrameElements(frameRaw, frameRate);
                }

                Debug.Log("try record" + needRecordedFrames.ToList().Count);
                foreach (var frameRaw in needRecordedFrames)
                {
                    FlaAnimationRecorder.RecordFrameElements(frameRaw, frameRate);
                }
               

                lastFrames.AddRange(needRecordedFrames);
                lastFrames.RemoveAll(e => needRemoveFrames.Any(r => r == e));
            }
            FlaAnimationRecorder.ApplyToClip(animationClip);
#endregion
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
    }
}
