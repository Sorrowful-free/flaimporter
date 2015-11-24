using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.Effects.Color
{
    public class FlaColorElementRaw
    {
        [XmlElement("Color")]
        public FlaColorRaw Color;
    }
}