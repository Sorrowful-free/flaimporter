using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.FillStyles.FillStyles.GradientFillStyles
{
    public class FlaGradientEntryRaw
    {
        [XmlAttribute("color")] 
        public string Color;

        [XmlAttribute("alpha")]
        public float Alpha;
        
        [XmlAttribute("ratio")]
        public float Ratio;
    }
}