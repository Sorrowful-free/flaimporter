using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.Geom;

namespace Assets.FlaExporter.Data.RawData.FillStyles.FillStyles
{
    public class FlaBaseFillStyleRaw
    {
        [XmlElement("matrix", Type = typeof(FlaMatrixElemetRaw))]
        public FlaMatrixElemetRaw Matrix;
    }
}