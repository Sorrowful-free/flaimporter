using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.Renderer.Enums;

namespace Assets.FlaExporter.FlaExporter.Renderer.FillStyles
{
    public static class FillStyleShadersNames
    {
        public static readonly Dictionary<FillStyleTypeEnum,string> ShaderNames = new Dictionary<FillStyleTypeEnum, string>
        {
            {FillStyleTypeEnum.SolidColor, "Fla/FillStyles/SolidColor"},
            {FillStyleTypeEnum.LinearGradient, "Fla/FillStyles/LinearGradient"},
            {FillStyleTypeEnum.RadialGradient, "Fla/FillStyles/RadialGradient"},
            {FillStyleTypeEnum.Bitmap, "Fla/FillStyles/Bitmap"},
        };
    }
}
