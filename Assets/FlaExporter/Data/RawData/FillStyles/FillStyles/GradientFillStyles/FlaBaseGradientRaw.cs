using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.Geom;

namespace Assets.FlaExporter.Data.RawData.FillStyles.FillStyles.GradientFillStyles
{
    public class FlaBaseGradientRaw : FlaBaseFillStyleRaw
    {
        [XmlAttribute("spreadMethod")]
        public string SpreadMethod;

        [XmlElement("matrix")]
        public FlaMatrixElemetRaw Matrix;

        [XmlArrayItem("GradientEntry")]
        public List<FlaGradientEntryRaw> GradientEntries;
    }
}
