using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.StorkeStyle
{
    public class FlaStorkeStyleRaw
    {
        [XmlAttribute("index")] 
        public int Index;

        [XmlElement("SolidStroke", typeof(FlaSolidStrokeStyleRaw))]
        public FlaBaseStorkyStyleRaw StorkyStyle;
    }
}