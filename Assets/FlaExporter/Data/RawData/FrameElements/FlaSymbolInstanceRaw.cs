using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.Effects.Color;
using Assets.FlaExporter.Data.RawData.Effects.Filters;
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

        [XmlAttribute("centerPoint3DX")]
        public float CenterPoint3Dx;

        [XmlAttribute("centerPoint3DY")] 
        public float CenterPoint3Dy;

        [XmlAttribute("rotationX")]
        public float RotationX;

        [XmlAttribute("rotationY")]
        public float RotationY;

        [XmlAttribute("blendMode")] 
        public string BlendMode;

        [XmlAttribute("cacheAsBitmap")] 
        public bool cacheAsBitmap;

        [XmlAttribute("referenceID")]
        public string ReferenceId;

        [XmlAttribute("matrix3D")]
        public string Matrix3D;

        [XmlElement("color")]
        public FlaColorElementRaw Color;

        [XmlArray("filters")]
        [XmlArrayItem("DropShadowFilter", Type = typeof(FlaDropShadowFilterRaw))]
        [XmlArrayItem("BlurFilter", Type = typeof(FlaBlurFilterRaw))]
        [XmlArrayItem("GlowFilter", Type = typeof(FlaGlowFilterRaw))]
        [XmlArrayItem("BevelFilter", Type = typeof(FlaBevelFilterRaw))]
        [XmlArrayItem("GradientGlowFilter", Type = typeof(FlaGradientGlowFilterRaw))]
        [XmlArrayItem("GradientBevelFilter", Type = typeof(FlaGradientBevelFilterRaw))]
        [XmlArrayItem("AdjustColorFilter", Type = typeof(FlaAdjustColorFilterRaw))]
        public List<FlaBaseFilterRaw> Filters;
    }
}