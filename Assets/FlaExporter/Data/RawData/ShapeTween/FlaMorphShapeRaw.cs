using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.ShapeTween
{
    public class FlaMorphShapeRaw
    {
        [XmlArray("morphSegments")] 
        [XmlArrayItem("MorphCurves")] 
        public List<FlaMorphSegmentRaw> MorphSegments;
    }
}