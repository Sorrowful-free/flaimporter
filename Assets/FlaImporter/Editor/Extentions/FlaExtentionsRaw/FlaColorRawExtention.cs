using System;
using Assets.FlaImporter.Editor.Data.RawData.Effects.Color;
using UnityEngine;

namespace Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw
{
    public static class FlaColorRawExtention
    {
        public static Color GetColorMultipler(this FlaColorRaw color)
        {
            return new Color(color.RedMultiplier,color.GreenMultiplier,color.BlueMultiplier,color.AlphaMultiplier);
        }

        public static Vector4 GetColorOffset(this FlaColorRaw color)
        {
            var f = new Func<int,float>((i) => { return (float) i/255.0f;});
            return new Vector4(f(color.RedOffset), f(color.GreenOffset), f(color.BlueOffset), f(color.AlphaOffset));
        }

    }
}
