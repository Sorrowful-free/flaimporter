using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.MotionTween
{
    public class FlaMotionObjectXMLRaw
    {
        [XmlElement("AnimationCore")] 
        public FlaAnimationCoreRaw AnimationCore;
    }
}