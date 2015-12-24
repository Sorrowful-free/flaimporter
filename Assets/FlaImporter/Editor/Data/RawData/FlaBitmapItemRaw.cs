using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData
{
    public class FlaBitmapItemRaw
    {
        [XmlAttribute("name")] 
        public string Name;
        
        [XmlAttribute("allowSmoothing")] 
        public bool AllowSmoothing;

        [XmlAttribute("href")] 
        public string Href;

        [XmlAttribute("frameRight")]
        public int FrameRight;

        [XmlAttribute("frameBottom")]
        public int FrameBottom;
    }
}