using System.Xml.Serialization;
using Assets.FlaImporter.Editor.Data.RawData.Effects.Color;
using Assets.FlaImporter.Editor.Data.RawData.Geom;

namespace Assets.FlaImporter.Editor.Data.RawData.FrameElements
{
    public class FlaFrameElementRaw
    {
        [XmlElement("matrix")]
        public FlaMatrixElemetRaw Matrix = new FlaMatrixElemetRaw();

        [XmlElement("transformationPoint")]
        public FlaTransformationPointElementRaw TransformationPoint = new FlaTransformationPointElementRaw();

        [XmlAttribute("centerPoint3DX")]
        public float CenterPoint3Dx;

        [XmlAttribute("centerPoint3DY")]
        public float CenterPoint3Dy;

        [XmlAttribute("centerPoint3DZ")]
        public float CenterPoint3Dz;

        [XmlAttribute("rotationX")]
        public float RotationX;

        [XmlAttribute("rotationY")]
        public float RotationY;

        [XmlAttribute("rotationZ")]
        public float RotationZ;

        [XmlAttribute("matrix3D")]
        public string Matrix3D;

        [XmlElement("color")]
        public FlaColorElementRaw Color = new FlaColorElementRaw();
    }
}