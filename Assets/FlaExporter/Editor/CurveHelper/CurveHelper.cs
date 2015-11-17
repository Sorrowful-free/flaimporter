using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.FlaExporter.Data.RawData;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.FlaExporter.Editor.CurveHelper
{
    internal class CurveHelper
    {
        
        public static void CurveCreate(GameObject documentGo, FlaTimeLineRaw flaTimeLine, string path = "Assets/Resources/Animations/")
        {

            #region chech directory

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            #endregion

            var clip = new AnimationClip();

            #region createCurveData

            foreach (var flaLayerRaw in flaTimeLine.Layers)
            {
                foreach (var flaFrameRaw in flaLayerRaw.Frames)
                {
                    foreach (var flaFrameElementRaw in flaFrameRaw.Elements)
                    {
                        
                    }
                }
            }
            clip.ClearCurves();
            var curveData = new AnimationClipCurveData();
            var curva = curveData.curve = new AnimationCurve();
            curveData.path = "имя объекта который нужно анимировать по тиму flatimeline.flalayer.flaraw.elemens.element.name";
            curveData.type = typeof (Transform);
            curveData.propertyName = "localTransform.x";
            curva.AddKey(0, 0);


            //что бы добавить в клип курву делаем такс
            clip.SetCurve(curveData.path, curveData.type, curveData.propertyName, curva);
            //естественно это должно быть внутри фор'Ычы. но клип должен быть один общий на все элементы, как я понял.
            #endregion



            #region Save
            
            AssetDatabase.CreateAsset(clip, path + documentGo.name + "/" + documentGo.name + "_animations/" + documentGo.name + "_" + clip.name + ".anim");
            var animatorController = AnimatorController.CreateAnimatorControllerAtPathWithClip(path + documentGo.name + "/" + documentGo.name + ".controller", clip);

            #endregion

            documentGo.AddComponent<Animator>().runtimeAnimatorController = animatorController;
        }
    }
}
