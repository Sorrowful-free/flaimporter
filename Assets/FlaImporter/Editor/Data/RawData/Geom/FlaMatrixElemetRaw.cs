using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.Geom
{
    public class FlaMatrixElemetRaw
    {
        [XmlElement("Matrix")] 
        public FlaMatrixRaw Matrix = new FlaMatrixRaw();
    }
}