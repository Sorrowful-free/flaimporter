using System.Xml.Serialization;
using Assets.FlaExporter.Editor.Data.RawData.Geom;

namespace Assets.FlaExporter.Editor.Data.RawData.FillStyles.FillStyles
{
    public class FlaBaseFillStyleRaw
    {
        [XmlElement("matrix", Type = typeof(FlaMatrixElemetRaw))]
        public FlaMatrixElemetRaw Matrix = new FlaMatrixElemetRaw();
    }
}