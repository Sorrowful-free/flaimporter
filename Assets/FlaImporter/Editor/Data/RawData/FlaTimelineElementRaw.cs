using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData
{
    public class FlaTimelineElementRaw
    {
        [XmlElement("DOMTimeline")]
        public FlaTimeLineRaw Timeline;
    }
}