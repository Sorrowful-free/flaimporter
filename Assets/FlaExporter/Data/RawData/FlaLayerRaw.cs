using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData
{
    public class FlaLayerRaw
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("color")] 
        public string Color;

        [XmlAttribute("autoNamed")]
        public bool AutoNamed;

        [XmlAttribute("animationType")]
        public string AnimationType;

        [XmlAttribute("current")] 
        public bool Current;

        [XmlAttribute("isSelected")] 
        public bool IsSelected;

        [XmlAttribute("locked")] 
        public bool Locked;

        [XmlAttribute("layerType")]
        public string LayerType;

        [XmlAttribute("parentLayerIndex")]
        public int ParentLayerIndex;

        [XmlArray("frames")] 
        [XmlArrayItem("DOMFrame")] 
        public List<FlaFrameRaw> Frames;

    }
}