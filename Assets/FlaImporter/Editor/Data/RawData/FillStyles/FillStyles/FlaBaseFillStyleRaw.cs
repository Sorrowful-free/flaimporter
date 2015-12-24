using System.Xml.Serialization;
using Assets.FlaImporter.Editor.Data.RawData.Geom;

namespace Assets.FlaImporter.Editor.Data.RawData.FillStyles.FillStyles
{
    public class FlaBaseFillStyleRaw
    {
        [XmlElement("Matrix4X4", Type = typeof(FlaMatrixElemetRaw))]
        public FlaMatrixElemetRaw Matrix = new FlaMatrixElemetRaw();
    }
}