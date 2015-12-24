using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaImporter.Editor.Data.RawData.FillStyles.FillStyles.GradientFillStyles;

namespace Assets.FlaImporter.Editor.Data.RawData.Effects.Filters
{
    public class FlaGradientBevelFilterRaw : FlaBaseFilterRaw
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

        [XmlAttribute("strength")]
        public float Strength;

        [XmlElement("GradientEntry", Type = typeof(FlaGradientEntryRaw))]
        public List<FlaGradientEntryRaw> GradientEntries;
    }
}