using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData
{
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
