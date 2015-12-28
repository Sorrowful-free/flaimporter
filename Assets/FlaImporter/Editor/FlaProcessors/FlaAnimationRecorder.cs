using System;
using System.Collections.Generic;
using Assets.FlaImporter.Editor.Data.RawData;
using Assets.FlaImporter.Editor.Extentions;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.Editor.Utils;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder;
using Assets.FlaImporter.FlaImporter.Transorm;
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

                float index = frameRaw.Elements.IndexOf(elementRaw);

                propAnim.Order.Record(index, time);
                //RecordHelper(propAnim.Order.Record, time, index,0);
               propAnim.Scale.Record(elementRaw.Matrix.Matrix.GetScale(), time);
                //RecordHelper(propAnim.Scale.Record, time, elementRaw.Matrix.Matrix.GetScale(),Vector2.one);
                propAnim.Position.Record(elementRaw.Matrix.Matrix.GetPosition(), time);
               // RecordHelper(propAnim.Position.Record, time, elementRaw.Matrix.Matrix.GetPosition(), Vector2.zero);
                propAnim.TransformPoint.Record(elementRaw.GetTransformPoint(), time);
                //RecordHelper(propAnim.TransformPoint.Record, time, elementRaw.GetTransformPoint(),Vector2.zero);
                propAnim.Rotation.Record(elementRaw.Matrix.Matrix.GetAngle(),time);
                //RecordHelper(propAnim.Rotation.Record, time, elementRaw.Matrix.Matrix.GetAngle(),0);
                propAnim.Skew.Record(elementRaw.Matrix.Matrix.GetSkew(), time);
                //RecordHelper(propAnim.Skew.Record, time, elementRaw.Matrix.Matrix.GetSkew(),Vector2.zero);
                propAnim.ColorMultipler.Record(elementRaw.Color.Color.GetColorMultipler(),time);
               // RecordHelper(propAnim.ColorMultipler.Record, time, (Vector4)elementRaw.Color.Color.GetColorMultipler(),Vector4.one);
                propAnim.ColorOffset.Record(elementRaw.Color.Color.GetColorOffset(), time);
               // RecordHelper(propAnim.ColorOffset.Record, time, elementRaw.Color.Color.GetColorOffset(),Vector4.zero);

                propAnim.Visible.Record(true,time);
            }
        }

        private static void RecordHelper<TType>(Action<TType,float> recordFunction, float time,TType value, TType defaultValue) where TType : struct 
        {
            if (!Equals(value, defaultValue))
            {
                recordFunction(value,time);
            }
        }

        public static void ReleaseFrameElements(FlaFrameRaw frameRaw, int framerate = 30)
        {
            foreach (var elementRaw in frameRaw.Elements)
            {
                var elementGO = FlaObjectManager.GetBusyObject(elementRaw);
                FlaObjectManager.ReleaseObject(elementGO);
            }

            foreach (var freeObject in FlaObjectManager.GetAllFreeObjects())
            {
                var propAnim = default(PropertyAnimationHolder);
                if (!_propertyAnimations.TryGetValue(freeObject.name, out propAnim))
                {
                    propAnim = new PropertyAnimationHolder(freeObject.name);
                    _propertyAnimations.Add(freeObject.name, propAnim);
                }
                var time = (float)((float)frameRaw.Index / (float)framerate);
             //   propAnim.Visible.Record(false, time);
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
        public readonly ColorCurve ColorOffset;
        public readonly ColorCurve ColorMultipler;

        public readonly FloatCurve Rotation;
        public readonly FloatCurve Order;
        public readonly Vector2Curve Position;
        public readonly Vector2Curve Scale;
        public readonly Vector2Curve Skew;
        public readonly Vector2Curve TransformPoint;

        public readonly BoolCurve Visible;

        public PropertyAnimationHolder(string relativePath)
        {
            RelativePath = relativePath;
            Order = new FloatCurve();
            Rotation = new FloatCurve();
            Position = new Vector2Curve();
            Scale = new Vector2Curve();
            Skew = new Vector2Curve();
            ColorOffset = new ColorCurve();
            ColorMultipler = new ColorCurve();
            TransformPoint = new Vector2Curve();
            Visible = new BoolCurve();
        }

        public void ApplyToClip(AnimationClip clip)
        {
            ColorOffset.ApplyToClip(clip, typeof(FlaColorAndFiltersHolder), RelativePath, "_selfColorTransform.ColorOffset",false);
            ColorMultipler.ApplyToClip(clip, typeof(FlaColorAndFiltersHolder), RelativePath, "_selfColorTransform.ColorMultipler",true);

            Order.ApplyToClip(clip, typeof(FlaTransform), RelativePath, "Order");
            Scale.ApplyToClip(clip, typeof(FlaTransform), RelativePath, "Scale");
            Position.ApplyToClip(clip, typeof(FlaTransform), RelativePath, "Position");
            TransformPoint.ApplyToClip(clip, typeof(FlaTransform), RelativePath, "TransformPoint");
            Rotation.ApplyToClip(clip, typeof(FlaTransform), RelativePath, "Rotation");
            Skew.ApplyToClip(clip,typeof(FlaTransform),RelativePath,"Skew");
            
            Visible.ApplyToClip(clip,typeof(GameObject),RelativePath,"m_IsActive");
        }
    }

    public class ColorCurve
    {
        
        public readonly AnimationCurve R;
        public readonly AnimationCurve G;
        public readonly AnimationCurve B;
        public readonly AnimationCurve A;

        public ColorCurve()
        {
            R = new AnimationCurve();
            G = new AnimationCurve();
            B = new AnimationCurve();
            A = new AnimationCurve();
        }

        public void Record(Vector4 vector,float time)
        {
            R.AddKey(time, vector.x);
            G.AddKey(time, vector.y);
            B.AddKey(time, vector.z);
            A.AddKey(time, vector.w);
        }

        public Vector4 GetValue(int index)
        {
            return new Vector4(R.keys[index].value, G.keys[index].value, B.keys[index].value, A.keys[index].value);
        }

        public void ApplyToClip(AnimationClip clip, Type targetType,string relativePath,string propertyName,bool isColor)
        {
            if (R.keys.Length > 0)
            {
                if (isColor)
                {
                    clip.SetCurve(relativePath, targetType, propertyName + ".r", R);
                    clip.SetCurve(relativePath, targetType, propertyName + ".g", G);
                    clip.SetCurve(relativePath, targetType, propertyName + ".b", B);
                    clip.SetCurve(relativePath, targetType, propertyName + ".a", A);   
                }
                else
                {
                    clip.SetCurve(relativePath, targetType, propertyName + ".x", R);
                    clip.SetCurve(relativePath, targetType, propertyName + ".y", G);
                    clip.SetCurve(relativePath, targetType, propertyName + ".z", B);
                    clip.SetCurve(relativePath, targetType, propertyName + ".w", A);  
                }
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
            if (X.keys.Length > 0)
            {
                X.SetCurveLinear();
                Y.SetCurveLinear();
                clip.SetCurve(relativePath, targetType, propertyName + ".x", X);
                clip.SetCurve(relativePath, targetType, propertyName + ".y", Y);    
            }
            
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
            clip.SetCurve(relativePath, targetType, propertyName, Flag); 
        }

    }

    public class FloatCurve
    {
        public readonly AnimationCurve Float;

        public FloatCurve()
        {
            Float = new AnimationCurve();
        }

        public void Record(float value, float time)
        {
            Float.AddKey(time, value);
        }

        public bool GetValue(int index)
        {
            return Float.keys[index].value > 0;
        }

        public void ApplyToClip(AnimationClip clip, Type targetType, string relativePath, string propertyName)
        {
            if (Float.keys.Length > 0)
            {
                clip.SetCurve(relativePath, targetType, propertyName, Float);
            }
        }

    }
}
