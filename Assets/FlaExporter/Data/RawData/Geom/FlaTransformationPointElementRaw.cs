using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.Geom
{
    public class FlaTransformationPointElementRaw
    {
        [XmlElement("Point")]
        public FlaPointRaw Point = new FlaPointRaw();
    }
}