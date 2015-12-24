using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.MotionTween
{
    public class FlaMotionObjectXMLRaw
    {
        [XmlElement("AnimationCore")] 
        public FlaAnimationCoreRaw AnimationCore;
    }
}