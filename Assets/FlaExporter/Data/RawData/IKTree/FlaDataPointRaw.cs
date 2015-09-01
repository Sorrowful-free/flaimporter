using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.IKTree
{
    public class FlaDataPointRaw
    {
        [XmlAttribute("integerPoint")]
        public string IntegerPoint;
    }
}