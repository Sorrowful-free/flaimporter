using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData
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
        public string AnimationType; // default, IK pose,motion object,guide, mask
        
        [XmlAttribute("current")] 
        public bool Current;

        [XmlAttribute("isSelected")] 
        public bool IsSelected;

        [XmlAttribute("locked")] 
        public bool Locked;

        [XmlAttribute("visible")]
        public bool Visible = true;

        [XmlAttribute("layerType")]
        public string LayerType;

        [XmlAttribute("parentLayerIndex")] 
        public int ParentLayerIndex = -1;

        [XmlArray("frames")] 
        [XmlArrayItem("DOMFrame")] 
        public List<FlaFrameRaw> Frames;
        
    }
}