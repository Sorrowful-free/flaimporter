using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.IKTree
{
    public class FlaIkContourRaw
    {
        [XmlAttribute("interior")]
        public bool Interior;

        [XmlAttribute("orientation")]
        public string Orientation;
        
        [XmlAttribute("rectLeft")]
        public int RectLeft;

        [XmlAttribute("rectTop")]
        public int RectTop;

        [XmlAttribute("rectRight")]
        public int RectRight;

        [XmlAttribute("rectBottom")]
        public int RectBottom;

        [XmlAttribute("fillSelected")]
        public bool FillSelected;

        [XmlAttribute("strokeHasSelection")]
        public bool StrokeHasSelection;

        [XmlAttribute("strokeFullySelected")]
        public bool StrokeFullySelected;

        [XmlArray("dataPoints")]
        [XmlArrayItem("DataPoint")]
        public List<FlaDataPointRaw> DataPoints;
    }
}