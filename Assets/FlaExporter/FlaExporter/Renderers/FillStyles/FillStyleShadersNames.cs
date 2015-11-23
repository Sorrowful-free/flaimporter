using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.Renderers.Enums;

namespace Assets.FlaExporter.FlaExporter.Renderers.FillStyles
{
    public static class FillStyleShadersNames
    {
        public readonly static Dictionary<FillStyleTypeEnum,string> ShaderNames = new Dictionary<FillStyleTypeEnum, string>
        {
            {FillStyleTypeEnum.SolidColor, "Fla/FillStyles/SolidColor"},
            {FillStyleTypeEnum.Bitmap, "Fla/FillStyles/Bitmap"},
            {FillStyleTypeEnum.LinearGradient, "Fla/FillStyles/LinearGradient"},
            {FillStyleTypeEnum.RadialGradient, "Fla/FillStyles/RadialGradient"},
        };
    }
}
