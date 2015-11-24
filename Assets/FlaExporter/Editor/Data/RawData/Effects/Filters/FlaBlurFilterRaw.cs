using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.Effects.Filters
{
    public class FlaBlurFilterRaw : FlaBaseFilterRaw
    {
        [XmlAttribute("blurX")]
        public float BlurX;

        [XmlAttribute("blurY")]
        public float BlurY;

        [XmlAttribute("distance")]
        public float Distance;

        [XmlAttribute("quality")]
        public float Quality;
    }
}