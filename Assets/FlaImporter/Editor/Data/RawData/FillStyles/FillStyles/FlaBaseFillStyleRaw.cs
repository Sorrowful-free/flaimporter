using System.Xml.Serialization;
using Assets.FlaImporter.Editor.Data.RawData.Geom;

namespace Assets.FlaImporter.Editor.Data.RawData.FillStyles.FillStyles
{
    public class FlaBaseFillStyleRaw
    {
        [XmlElement("matrix", Type = typeof(FlaMatrixElemetRaw))]
        public FlaMatrixElemetRaw Matrix = new FlaMatrixElemetRaw();
    }
}