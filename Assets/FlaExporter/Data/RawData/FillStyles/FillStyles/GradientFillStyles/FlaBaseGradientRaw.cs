using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.Geom;

namespace Assets.FlaExporter.Data.RawData.FillStyles.FillStyles.GradientFillStyles
{
    public class FlaBaseGradientRaw : FlaBaseFillStyleRaw
    {
        [XmlAttribute("spreadMethod")]
        public string SpreadMethod;

        [XmlElement("matrix", Type = typeof(FlaMatrixElemetRaw))]
        public FlaMatrixElemetRaw Matrix;

        [XmlElement("GradientEntry", Type = typeof(FlaGradientEntryRaw))]
        public List<FlaGradientEntryRaw> GradientEntries;

    }
}
