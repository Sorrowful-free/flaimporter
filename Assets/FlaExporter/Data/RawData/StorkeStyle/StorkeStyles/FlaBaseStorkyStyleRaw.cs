using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.StorkeStyle.StorkeStyles
{
    public class FlaBaseStorkyStyleRaw
    {
        [XmlAttribute("scaleMode")]
        public string ScaleMode;
        
        [XmlAttribute("weight")]
        public float Weigth;
        
        [XmlElement("fill")]
        public FlaStorkeFillRaw Fill;

        [XmlArray("VariablePointWidth")]
        [XmlArrayItem("WidthMarker")]
        public List<FlaVariablePointWidthRaw> VariablePointWidth;

        [XmlAttribute("caps")]
        public string Caps;

        [XmlAttribute("joints")]
        public string Joints;

        [XmlAttribute("pixelHinting")]
        public bool PixelHinting;

        [XmlAttribute("solidStyle")]
        public string SolidStyle;
    }
}