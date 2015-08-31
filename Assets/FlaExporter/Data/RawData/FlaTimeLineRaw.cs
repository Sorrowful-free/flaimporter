using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData
{
    public class FlaTimeLineRaw
    {
        [XmlAttribute("name")] 
        public string Name;

        [XmlAttribute("currentFrame")] 
        public int CurrentFrame;

        [XmlArray("layers")]
        [XmlArrayItem("DOMLayer")]
        public List<FlaLayerRaw> Layers;

        public override string ToString()
        {
            return string.Format("name:{0}, currentFrame:{1}", Name, CurrentFrame);
        }
    }
}