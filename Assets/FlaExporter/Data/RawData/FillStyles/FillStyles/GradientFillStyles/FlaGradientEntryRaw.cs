using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.FillStyles.FillStyles.GradientFillStyles
{
    public class FlaGradientEntryRaw
    {
        [XmlAttribute("color")] 
        public string Color;

        [XmlAttribute("ratio")]
        public float Ratio;
    }
}