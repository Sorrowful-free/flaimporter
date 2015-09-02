using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData
{
    public class FlaTimelineElementRaw
    {
        [XmlElement("DOMTimeline")]
        public FlaTimeLineRaw Timeline;
    }
}