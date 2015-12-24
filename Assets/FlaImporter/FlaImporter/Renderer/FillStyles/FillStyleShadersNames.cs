using System.Collections.Generic;
using Assets.FlaImporter.FlaImporter.Renderer.Enums;

namespace Assets.FlaImporter.FlaImporter.Renderer.FillStyles
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
