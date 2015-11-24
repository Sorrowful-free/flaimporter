using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.ShapeTween
{
    public class FlaMorphSegmentRaw
    {
        [XmlAttribute("startPointA")]
        public string StartPointA;

        [XmlAttribute("startPointB")]
        public string StartPointB;

        [XmlAttribute("strokeIndex1")]
        public int StrokeIndex1;

        [XmlAttribute("strokeIndex2")]
        public int StrokeIndex2;

        [XmlAttribute("fillIndex1")]
        public int FillIndex1;

        [XmlAttribute("fillIndex2")]
        public int FillIndex2;
        
        [XmlElement("MorphCurves", Type = typeof(FlaMorphCurvesRaw))]
        public List<FlaMorphCurvesRaw> MorphCurves;
    }
}