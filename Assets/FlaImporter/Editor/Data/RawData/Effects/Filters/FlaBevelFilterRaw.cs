using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.Effects.Filters
{
    public class FlaBevelFilterRaw : FlaBaseFilterRaw
    {
        [XmlAttribute("blurX")]
        public float BlurX;

        [XmlAttribute("blurY")]
        public float BlurY;

        [XmlAttribute("distance")]
        public float Distance;

        [XmlAttribute("quality")]
        public float Quality;

        [XmlAttribute("knockout")]
        public bool Knockout;

        [XmlAttribute("type")]
        public string Type;
    }
}