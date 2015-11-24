using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.Geom
{
    public class FlaTransformationPointElementRaw
    {
        [XmlElement("Point")]
        public FlaPointRaw Point = new FlaPointRaw();
    }
}