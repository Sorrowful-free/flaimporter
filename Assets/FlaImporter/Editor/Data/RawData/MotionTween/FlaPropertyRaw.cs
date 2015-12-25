using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.MotionTween
{
    public class FlaPropertyRaw
    {
        //enabled="1" id="Motion_X" ignoreTimeMap="0" readonly="0" visible="1"
        [XmlAttribute("enabled")]
        public int Enabled;

        [XmlAttribute("id")]
        public string Id;

        [XmlAttribute("ignoreTimeMap")]
        public int IgnoreTimeMap;

        [XmlAttribute("readonly")]
        public int Readonly;

        [XmlAttribute("visible")]
        public int Visible;

        [XmlElement("Keyframe", Type = typeof(FlaKeyFrameRaw))]
        public List<FlaKeyFrameRaw> KeyFrames;
    }
}