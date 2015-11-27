using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.Effects.Color
{
    public class FlaColorRaw
    {
        [XmlAttribute("brightness")]
        public float Brightness;

        [XmlAttribute("tintMultiplier")]
        public float TintMultiplier;

        [XmlAttribute("tintColor")]
        public string TintColor;

        [XmlAttribute("alphaMultiplier")]
        public float AlphaMultiplier = 1;

        [XmlAttribute("blueMultiplier")]
        public float BlueMultiplier = 1;

        [XmlAttribute("greenMultiplier")]
        public float GreenMultiplier = 1;

        [XmlAttribute("redMultiplier")]
        public float RedMultiplier = 1;

        [XmlAttribute("alphaOffset")]
        public int AlphaOffset = 0;

        [XmlAttribute("blueOffset")]
        public int BlueOffset = 0;

        [XmlAttribute("greenOffset")]
        public int GreenOffset = 0;

        [XmlAttribute("redOffset")]
        public int RedOffset = 0;

    }
}