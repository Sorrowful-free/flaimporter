using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.Effects.Filters
{
    public class FlaDropShadowFilterRaw : FlaBaseFilterRaw
    {
        [XmlAttribute("blurX")]
        public float BlurX;

        [XmlAttribute("blurY")]
        public float BlurY;

        [XmlAttribute("distance")]
        public float Distance;

        [XmlAttribute("quality")]
        public float Quality;
       
        [XmlAttribute("knockout")] 
        public bool Knockout;

        [XmlAttribute("inner")]
        public bool Inner;

        [XmlAttribute("hideObject")]
        public bool HideObject;

    }
}