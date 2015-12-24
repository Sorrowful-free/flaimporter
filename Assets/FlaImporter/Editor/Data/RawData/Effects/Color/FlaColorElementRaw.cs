using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.Effects.Color
{
    public class FlaColorElementRaw
    {
        [XmlElement("Color")]
        public FlaColorRaw Color = new FlaColorRaw();
    }
}