using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.Effects.Filters
{
    public class FlaAdjustColorFilterRaw : FlaBaseFilterRaw
    {
        [XmlAttribute("brightness")]
        public int Brightness;

        [XmlAttribute("contrast")]
        public int Contrast;

        [XmlAttribute("saturation")]
        public int Saturation;

        [XmlAttribute("hue")]
        public int Hue;
    }
}