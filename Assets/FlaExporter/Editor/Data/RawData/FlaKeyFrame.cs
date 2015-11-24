using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData
{
    public class FlaKeyFrame
    {
        //<Keyframe anchor="0,0" next="0,0" previous="0,0" roving="0" timevalue="0"/>
        [XmlAttribute("anchor")]
        public string Anchor;

        [XmlAttribute("next")]
        public string Next;

        [XmlAttribute("previous")]
        public string Previous;

        [XmlAttribute("roving")]
        public int Roving;

        [XmlAttribute("timevalue")]
        public int Timevalue;
    }
}