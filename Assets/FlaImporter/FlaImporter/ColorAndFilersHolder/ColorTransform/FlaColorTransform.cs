using System;
using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform.Enums;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform
{
    [Serializable]
    public struct FlaColorTransform 
    {
        public static readonly Dictionary<FlaColorTransformPropertyTypeEnum,string> PropertyNames = new Dictionary<FlaColorTransformPropertyTypeEnum, string>
        {
            {FlaColorTransformPropertyTypeEnum.ColorMultiplyR, "ColorMultipler.r"},
            {FlaColorTransformPropertyTypeEnum.ColorMultiplyG, "ColorMultipler.g"},
            {FlaColorTransformPropertyTypeEnum.ColorMultiplyB, "ColorMultipler.b"},
            {FlaColorTransformPropertyTypeEnum.ColorMultiplyA, "ColorMultipler.a"},
            {FlaColorTransformPropertyTypeEnum.ColorOffsetR, "ColorOffset.x"},
            {FlaColorTransformPropertyTypeEnum.ColorOffsetG, "ColorOffset.y"},
            {FlaColorTransformPropertyTypeEnum.ColorOffsetB, "ColorOffset.z"},
            {FlaColorTransformPropertyTypeEnum.ColorOffsetA, "ColorOffset.w"},
        };

        [SerializeField] 
        public Vector4 ColorOffset;//= new Vector4(0, 0, 0, 0);
        private Vector4 _oldColorOffset;// = new Vector4(0, 0, 0, 0);
       
        [SerializeField]
        public Color ColorMultipler;// = Color.white;
        private Color _oldColorMultipler;// = Color.white;

        [Range(-1, 1)]
        [SerializeField]
        public float BrightnessMultipler;// = 0;
        private float _oldBrightnessMultipler;// = 0;

        [Range(0,1)]
        [SerializeField]
        public float TintMultipler;// = 0;
        private float _oldTintMultipler;// = 0;

        [SerializeField]
        public Color TintColor;// = new Color(0, 0, 0, 0);
        private Color _oldTintColor;// = new Color(0, 0, 0, 0);
        
        public FlaColorTransform(Vector4 colorOffset, Color colorMultipler, Color tintColor, float tintMultipler, float brightnessMultipler)
        {
            _oldColorOffset = ColorOffset = colorOffset;
            _oldColorMultipler = ColorMultipler = colorMultipler;
            _oldTintColor = TintColor = tintColor;
            _oldTintMultipler = TintMultipler = tintMultipler;
            _oldBrightnessMultipler = BrightnessMultipler = brightnessMultipler;
        }
        
        public bool UpdateTint()
        {
            var flag = TintMultipler != _oldTintMultipler || TintColor != _oldTintColor;
            if (flag)
            {
                _oldTintMultipler = TintMultipler;
                _oldTintColor = TintColor;
                ApplyTint();
            }
            return flag;
        }

        public bool UpdateBrightness()
        {
            var flag = BrightnessMultipler != _oldBrightnessMultipler;
            if (flag)
            {
                _oldBrightnessMultipler = BrightnessMultipler;
                ApplyBrightness();
            }
            return flag;
        }

        private void ApplyBrightness()
        {
            if(BrightnessMultipler > 1)
            {
               BrightnessMultipler = 1;
            }
            else if(BrightnessMultipler < -1)
            {
               BrightnessMultipler = -1;
            }
            var percent = 1.0f - Math.Abs(BrightnessMultipler);
            var offset = 0.0f;
            if (BrightnessMultipler > 0)
            {
                offset = BrightnessMultipler;
            }
            ColorMultipler.r = ColorMultipler.g = ColorMultipler.b = percent;
            ColorOffset.x = ColorOffset.y = ColorOffset.z = offset;
        }

        private void ApplyTint()
        {
            ColorMultipler.r = ColorMultipler.g = ColorMultipler.b = 1 - TintMultipler;
            ColorOffset.x = TintColor.r * TintMultipler;
            ColorOffset.y = TintColor.g * TintMultipler;
            ColorOffset.z = TintColor.b * TintMultipler;
        }

        public bool UpdateColorTransform()
        {
            var flag = ColorOffset != _oldColorOffset || ColorMultipler != _oldColorMultipler;
            if (flag)
            {
                _oldColorOffset = ColorOffset;
                _oldColorMultipler = ColorMultipler;
            }
            return flag;
        }

        public void Concat(FlaColorTransform other)
        {
            ColorOffset.x = ColorOffset.x + ColorMultipler.r * ColorOffset.x;
            ColorOffset.y = ColorOffset.y + ColorMultipler.g * ColorOffset.y;
            ColorOffset.z = ColorOffset.z + ColorMultipler.b * ColorOffset.z;
            ColorOffset.w = ColorOffset.w + ColorMultipler.a * ColorOffset.w;
            ColorMultipler = ColorMultipler*other.ColorMultipler;
        }

        public void CopyFrom(FlaColorTransform other)
        {
            BrightnessMultipler = other.BrightnessMultipler;
            TintMultipler = other.TintMultipler;
            TintColor = other.TintColor;
            ColorOffset = other.ColorOffset;
            ColorMultipler = other.ColorMultipler;
        }

        public static FlaColorTransform operator *(FlaColorTransform colorTransform1, FlaColorTransform colorTransform2)
        {
            var temp = new FlaColorTransform();
            temp.CopyFrom(colorTransform1);
            temp.Concat(colorTransform2);
            return temp;
        }


    }
}
