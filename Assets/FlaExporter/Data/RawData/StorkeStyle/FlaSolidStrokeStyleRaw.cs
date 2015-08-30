using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.StorkeStyle
{
    public class FlaSolidStrokeStyleRaw : FlaBaseStorkyStyleRaw
    {
        [XmlElement("fill")] 
        public FlaStorkeFillRaw Fill;
    }
}