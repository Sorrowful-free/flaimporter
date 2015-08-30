using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.FrameElements;

namespace Assets.FlaExporter.Data.RawData
{
    public class FlaFrameRaw
    {
        [XmlAttribute("index")]
        public int Index;

        [XmlAttribute("duration")] 
        public int Duration;

        [XmlAttribute("tweenType")] 
        public string TweenType;

        [XmlAttribute("keyMode")] 
        public int KeyMode;

        [XmlAttribute("isIKPose")] 
        public bool IsIKPose;

        [XmlAttribute("poseLocations")]
        public string PoseLocation;

        [XmlArray("elements")]
        [XmlArrayItem("DOMShape",Type = typeof(FlaShapeRaw))]
        [XmlArrayItem("DOMSymbolInstance", Type = typeof(FlaSymbolInstanceRaw))]
        public List<FlaFrameElementRaw> Elements;
    }
}