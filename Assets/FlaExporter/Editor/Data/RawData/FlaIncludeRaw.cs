using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData
{
    public class FlaIncludeRaw
    {
        [XmlAttribute("href")]
        public string Href;
        
        [XmlAttribute("itemIcon")]
        public int ItemIcon;
        
        [XmlAttribute("itemID")]
        public string ItemId;
        
        [XmlAttribute("lastModified")] 
        public int LastModified;

    }
}