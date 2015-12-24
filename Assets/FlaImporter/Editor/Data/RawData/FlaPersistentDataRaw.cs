using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData
{
    public class FlaPersistentDataRaw
    {
        [XmlAttribute("n")]
        public string Name;
        [XmlAttribute("t")]
        public string Type;
        [XmlAttribute("v")]
        public int Version;
    }
}