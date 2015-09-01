using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData
{
    [XmlRoot("DOMSymbolItem", Namespace = "http://ns.adobe.com/xfl/2008/")]
    public class FlaSymbolItemRaw
    {
        [XmlAttribute("name")] 
        public string Name;

        [XmlAttribute("symbolType")]
        public string SymbolType;

        [XmlElement("timeline")]
        public FlaTimelineElementRaw Timeline;
    }
}
