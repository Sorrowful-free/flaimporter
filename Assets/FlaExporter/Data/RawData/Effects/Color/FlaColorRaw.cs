using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.Effects.Color
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
        public float AlphaMultiplier;

        [XmlAttribute("blueMultiplier")]
        public float BlueMultiplier;

        [XmlAttribute("greenMultiplier")]
        public float GreenMultiplier;

        [XmlAttribute("redMultiplier")]
        public float RedMultiplier;

        [XmlAttribute("blueOffset")]
        public int BlueOffset;

        [XmlAttribute("greenOffset")]
        public int GreenOffset;

        [XmlAttribute("redOffset")]
        public int RedOffset;
    }
}