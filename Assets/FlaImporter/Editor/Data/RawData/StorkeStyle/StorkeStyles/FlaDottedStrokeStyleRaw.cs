using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.StorkeStyle.StorkeStyles
{
    public class FlaDottedStrokeStyleRaw : FlaBaseStorkyStyleRaw
    {
        [XmlAttribute("dotSpace")] 
        public float DotSpace;
    }
}
