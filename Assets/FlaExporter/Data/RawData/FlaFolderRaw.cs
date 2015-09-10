using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData
{
    public class FlaFolderRaw
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("itemID")]
        public string ItemId;
        [XmlAttribute("isExpanded")]
        public bool IsExpanded;
    }
}