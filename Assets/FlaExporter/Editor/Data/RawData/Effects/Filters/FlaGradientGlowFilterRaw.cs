using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaExporter.Editor.Data.RawData.FillStyles.FillStyles.GradientFillStyles;

namespace Assets.FlaExporter.Editor.Data.RawData.Effects.Filters
{
    public class FlaGradientGlowFilterRaw : FlaBaseFilterRaw
    {
        [XmlAttribute("blurX")]
        public float BlurX;

        [XmlAttribute("blurY")]
        public float BlurY;

        [XmlAttribute("distance")]
        public float Distance;

        [XmlAttribute("quality")] 
        public float Quality;
       
        [XmlAttribute("type")]
        public string Type;

        [XmlElement("GradientEntry",Type = typeof(FlaGradientEntryRaw))]
        public List<FlaGradientEntryRaw> GradientEntries;
    }
}