using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.FillStyles.FillStyles.GradientFillStyles
{
    public class FlaBaseGradientRaw : FlaBaseFillStyleRaw
    {
        [XmlAttribute("spreadMethod")]
        public string SpreadMethod ="";
        
        [XmlElement("GradientEntry", Type = typeof(FlaGradientEntryRaw))]
        public List<FlaGradientEntryRaw> GradientEntries = new List<FlaGradientEntryRaw>();

        public override int GetHashCode()
        {
            return (SpreadMethod.GetHashCode() + GradientEntries.Sum(e => e.GetHashCode()/1000000)).GetHashCode();
        }
    }
}
