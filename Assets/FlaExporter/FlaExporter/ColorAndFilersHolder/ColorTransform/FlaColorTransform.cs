using System;
using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform.Enums;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform
{
    [Serializable]
    public class FlaColorTransform
    {
        public static readonly Dictionary<FlaColorTransformPropertyTypeEnum,string> PropertyNames = new Dictionary<FlaColorTransformPropertyTypeEnum, string>
        {
            {FlaColorTransformPropertyTypeEnum.ColorMultiplyR, "ColorMultipler.r"},
            {FlaColorTransformPropertyTypeEnum.ColorMultiplyG, "ColorMultipler.g"},
            {FlaColorTransformPropertyTypeEnum.ColorMultiplyB, "ColorMultipler.b"},
            {FlaColorTransformPropertyTypeEnum.ColorMultiplyA, "ColorMultipler.a"},
            {FlaColorTransformPropertyTypeEnum.ColorOffsetR, "ColorOffset.r"},
            {FlaColorTransformPropertyTypeEnum.ColorOffsetG, "ColorOffset.g"},
            {FlaColorTransformPropertyTypeEnum.ColorOffsetB, "ColorOffset.b"},
            {FlaColorTransformPropertyTypeEnum.ColorOffsetA, "ColorOffset.a"},
        };

        [SerializeField]
        public Color ColorOffset = new Color(0, 0, 0, 0);
        private Color _oldColorOffset = new Color(0, 0, 0, 0);
        [SerializeField]
        public Color ColorMultipler = Color.white;
        private Color _oldColorMultipler = Color.white;

        public bool IsChange
        {
            get
            {   var flag = ColorOffset != _oldColorOffset || ColorMultipler != _oldColorMultipler;
                if (flag)
                {
                    _oldColorOffset = ColorOffset;
                    _oldColorMultipler = ColorMultipler;
                }
                return flag;
            }
        }

        public void Concat(FlaColorTransform other)
        {
            ColorOffset = ColorOffset + ColorMultipler*other.ColorOffset;
            ColorMultipler = ColorMultipler*other.ColorMultipler;
        }

        public static FlaColorTransform operator *(FlaColorTransform colorTransform1, FlaColorTransform colorTransform2)
        {
            var temp = new FlaColorTransform
            {
                ColorOffset = colorTransform1.ColorOffset,
                ColorMultipler = colorTransform1.ColorMultipler
            };
            temp.Concat(colorTransform2);
            return temp;
        }
    }
}
