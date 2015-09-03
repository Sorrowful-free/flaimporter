using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Data.RawData.Geom;
using Assets.FlaExporter.Data.RawData.MotionTween;
using Assets.FlaExporter.Data.RawData.ShapeTween;

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

        [XmlAttribute("labelType")]
        public string LabelType;

        [XmlAttribute("motionTweenSnap")]
        public bool MotionTweenSnap;

        [XmlAttribute("poseLocations")]
        public string PoseLocation;

        [XmlAttribute("name")]
        public string Name;

        [XmlArray("elements")]
        [XmlArrayItem("DOMShape",Type = typeof(FlaShapeRaw))]
        [XmlArrayItem("DOMSymbolInstance", Type = typeof(FlaSymbolInstanceRaw))]
        [XmlArrayItem("DOMBitmapInstance", Type = typeof(FlaBitmapItemRaw))]
        public List<FlaFrameElementRaw> Elements;

        [XmlElement("MorphShape")] 
        public FlaMorphShapeRaw MorphShape;

        [XmlAttribute("motionTweenRotate")]
        public string MotionTweenRotate;

        [XmlAttribute("motionTweenScale")] 
        public bool MotionTweenScale;

        [XmlAttribute("isMotionObject")] 
        public bool IsMotionObject;

        [XmlAttribute("visibleAnimationKeyframes")] 
        public int VisibleAnimationKeyframes;

        [XmlElement("motionObjectXML")]
        public FlaMotionObjectXMLRaw MotionObjectXml;

        [XmlArray("betweenFrameMatrixList")]
        [XmlArrayItem("Matrix")]
        public List<FlaMatrixRaw> BetweenFrameMatrixList;

    }
}