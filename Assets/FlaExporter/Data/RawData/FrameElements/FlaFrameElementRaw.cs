using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.Geom;

namespace Assets.FlaExporter.Data.RawData.FrameElements
{
    public class FlaFrameElementRaw
    {
        [XmlElement("matrix")]
        public FlaMatrixElemetRaw Matrix = new FlaMatrixElemetRaw();

        [XmlElement("transformationPoint")]
        public FlaTransformationPointElementRaw TransformationPoint = new FlaTransformationPointElementRaw();
    }
}