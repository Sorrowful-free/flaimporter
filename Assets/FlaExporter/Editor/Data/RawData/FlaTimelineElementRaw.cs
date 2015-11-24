using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData
{
    public class FlaTimelineElementRaw
    {
        [XmlElement("DOMTimeline")]
        public FlaTimeLineRaw Timeline;
    }
}