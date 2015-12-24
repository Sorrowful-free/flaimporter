using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.Geom
{
    public class FlaPointRaw
    {
        [XmlAttribute("x")] 
        public float X;

        [XmlAttribute("y")] 
        public float Y;
    }
}