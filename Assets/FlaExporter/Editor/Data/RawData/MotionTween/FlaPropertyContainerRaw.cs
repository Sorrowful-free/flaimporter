using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.MotionTween
{
    public class FlaPropertyContainerRaw
    {
        [XmlAttribute("id")]
        public string Id;

        [XmlElement("PropertyContainer", Type = typeof (FlaPropertyContainerRaw))] 
        public List<FlaPropertyContainerRaw> PropertyContainers;

        [XmlElement("Property", Type = typeof(FlaPropertyRaw))]
        public List<FlaPropertyRaw> Properties;
    }
}