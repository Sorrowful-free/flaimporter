using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.StorkeStyle.StorkeStyles
{
    public class FlaDottedStrokeStyleRaw : FlaBaseStorkyStyleRaw
    {
        [XmlAttribute("dotSpace")] 
        public float DotSpace;
    }
}
