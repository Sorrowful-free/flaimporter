using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData
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