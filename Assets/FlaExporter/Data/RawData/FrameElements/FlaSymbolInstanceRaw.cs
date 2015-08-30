using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.Geom;

namespace Assets.FlaExporter.Data.RawData.FrameElements
{
    public class FlaSymbolInstanceRaw : FlaFrameElementRaw
    {
        [XmlAttribute("libraryItemName")]
        public string LibraryItemName;
        [XmlAttribute("selected")]
        public bool Selected;

        [XmlElement("matrix")] 
        public FlaMatrixElemetRaw Matrix;

        [XmlElement("transformationPoint")] 
        public FlaTransformationPointElementRaw TransformationPoint;
    }
}