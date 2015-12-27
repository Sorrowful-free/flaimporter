using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.FlaImporter.Editor.Data.RawData;
using Assets.FlaImporter.Editor.Extentions;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.Editor.Utils;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder;
using Assets.FlaImporter.FlaImporter.Transorm;
using Assets.FlaImporter.FlaImporter.Transorm.Enums;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaAnimationRecorder
    {

        private readonly static Dictionary<string, PropertyAnimationHolder> _propertyAnimations = new Dictionary<string, PropertyAnimationHolder>();
        public static void RecordFrameElements(FlaFrameRaw frameRaw, int framerate = 30)
        {
            foreach (var elementRaw in frameRaw.Elements)
            {
                var elementGO = FlaObjectManager.GetFreeObject(elementRaw);
                var propAnim = default(PropertyAnimationHolder);
                if (!_propertyAnimations.TryGetValue(elementGO.name,out propAnim))
                {
                    propAnim = new PropertyAnimationHolder(elementGO.name);
                    _propertyAnimations.Add(elementGO.name,propAnim);
                }
                var time = (float) ((float) frameRaw.Index/(float) framerate);
                
                Debug.Log("try record matrix "+ elementRaw.Matrix.Matrix);
                propAnim.ABCD.Record(elementRaw.Matrix.Matrix.GetABCD(), time);
                propAnim.TXTY.Record(elementRaw.Matrix.Matrix.GetTXTY()/FlaImporterConstatns.PixelsPerUnits, time);
                var tranfrormPoint = new Vector2(elementRaw.GetTransformValueByPropertyType(FlaTransformPropertyTypeEnum.TransformPointX), elementRaw.GetTransformValueByPropertyType(FlaTransformPropertyTypeEnum.TransformPointY));
                propAnim.TransformPoint.Record(tranfrormPoint, time);

                propAnim.TXTY.Record(elementRaw.Matrix.Matrix.GetTXTY() / FlaImporterConstatns.PixelsPerUnits, time);
                propAnim.ColorMultipler.Record(elementRaw.Color.Color.GetColorMultipler(),time);
                propAnim.ColorMultipler.Record(elementRaw.Color.Color.GetColorOffset(), time);

                propAnim.Visible.Record(true,time);
            }
        }

        public static void ReleaseFrameElements(FlaFrameRaw frameRaw, int framerate = 30)
        {
            foreach (var elementRaw in frameRaw.Elements)
            {
                var elementGO = FlaObjectManager.GetBusyObject(elementRaw);
                var propAnim = default(PropertyAnimationHolder);
                if (!_propertyAnimations.TryGetValue(elementGO.name, out propAnim))
                {
                    propAnim = new PropertyAnimationHolder(elementGO.name);
                    _propertyAnimations.Add(elementGO.name, propAnim);
                }
                var time = (float)((float)frameRaw.Index / (float)framerate);
                
                propAnim.Visible.Record(false, time);
                FlaObjectManager.ReleaseObject(elementGO);
            }
        }

        public static void ApplyToClip(AnimationClip clip)
        {
            foreach (var propertyAnimation in _propertyAnimations)
            {
                propertyAnimation.Value.ApplyToClip(clip); 
            }
        }

        public static void Clear()
        {
            _propertyAnimations.Clear();
        }

    }

    public class PropertyAnimationHolder
    {
        public readonly string RelativePath;
        public readonly Vector4Curve ColorOffset;
        public readonly Vector4Curve ColorMultipler;

        public readonly Vector4Curve ABCD;
        public readonly Vector2Curve TXTY;

        public readonly Vector2Curve TransformPoint;

        public readonly BoolCurve Visible;

        public PropertyAnimationHolder(string relativePath)
        {
            Debug.Log(string.Format("craete anim holder rp{0}",relativePath));
            RelativePath = relativePath;
            ColorOffset = new Vector4Curve();
            ColorMultipler = new Vector4Curve();
            ABCD = new Vector4Curve();
            TXTY = new Vector2Curve();
            TransformPoint = new Vector2Curve();
            Visible = new BoolCurve();
        }

        public void ApplyToClip(AnimationClip clip)
        {
            ColorOffset.ApplyToClip(clip, typeof(FlaColorAndFiltersHolder), RelativePath, "_selfColorTransform.ColorOffset", false);
            ColorMultipler.ApplyToClip(clip, typeof(FlaColorAndFiltersHolder), RelativePath, "_selfColorTransform.ColorMultipler", true);

            ABCD.ApplyToClip(clip, typeof(FlaTransform), RelativePath, "Matrix2D.ABCD", false);
            TXTY.ApplyToClip(clip, typeof(FlaTransform), RelativePath, "Matrix2D.TXTY");

            TransformPoint.ApplyToClip(clip, typeof(FlaTransform), RelativePath, "TransformPoint");
            
            Visible.ApplyToClip(clip,typeof(GameObject),RelativePath,"m_IsActive");
        }
    }

    public class Vector4Curve
    {
        
        public readonly AnimationCurve X;
        public readonly AnimationCurve Y;
        public readonly AnimationCurve Z;
        public readonly AnimationCurve W;

        public Vector4Curve()
        {
            X = new AnimationCurve();
            Y = new AnimationCurve();
            Z = new AnimationCurve();
            W = new AnimationCurve();
        }

        public void Record(Vector4 vector,float time)
        {
            X.AddKey(time, vector.x);
            Y.AddKey(time, vector.y);
            Z.AddKey(time, vector.z);
            W.AddKey(time, vector.w);
        }

        public Vector4 GetValue(int index)
        {
            return new Vector4(X.keys[index].value, Y.keys[index].value, Z.keys[index].value, W.keys[index].value);
        }

        public void ApplyToClip(AnimationClip clip, Type targetType,string relativePath,string propertyName, bool isColor)
        {
            if (!isColor)
            {
                clip.SetCurve(relativePath,targetType,propertyName+".x",X);
                clip.SetCurve(relativePath,targetType,propertyName+".y",Y);
                clip.SetCurve(relativePath,targetType,propertyName+".z",Z);
                clip.SetCurve(relativePath,targetType,propertyName+".w",W);
            }
            else
            {
                clip.SetCurve(relativePath,targetType,propertyName+".r",X);
                clip.SetCurve(relativePath,targetType,propertyName+".g",Y);
                clip.SetCurve(relativePath,targetType,propertyName+".b",Z);
                clip.SetCurve(relativePath,targetType,propertyName+".a",W);
            }
        }
    }

    public class Vector2Curve
    {

        public readonly AnimationCurve X;
        public readonly AnimationCurve Y;

        public Vector2Curve()
        {
            X = new AnimationCurve();
            Y = new AnimationCurve();
        }

        public void Record(Vector2 vector, float time)
        {
            X.AddKey(time, vector.x);
            Y.AddKey(time, vector.y);
        }

        public Vector2 GetValue(int index)
        {
            return new Vector2(X.keys[index].value, Y.keys[index].value);
        }

        public void ApplyToClip(AnimationClip clip, Type targetType, string relativePath, string propertyName)
        {
            clip.SetCurve(relativePath, targetType, propertyName + ".x", X);
            clip.SetCurve(relativePath, targetType, propertyName + ".y", Y);
        }
    }

    public class BoolCurve
    {
       public readonly AnimationCurve Flag;

        public BoolCurve()
        {
            Flag = new AnimationCurve();
        }

        public void Record(bool value,float time)
        {
            Flag.AddKey(time, !value?0:1);
        }

        public bool GetValue(int index)
        {
            return Flag.keys[index].value > 0;
        }

        public void ApplyToClip(AnimationClip clip, Type targetType,string relativePath,string propertyName)
        {
            Flag.SetCurveLinear();
            AnimationUtility.
            clip.SetCurve(relativePath, targetType, propertyName, Flag);
        }

    }
}
