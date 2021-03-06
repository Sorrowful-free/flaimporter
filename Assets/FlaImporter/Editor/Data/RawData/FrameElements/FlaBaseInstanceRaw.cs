﻿using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.FlaImporter.Editor.Data.RawData.Effects.Filters;

namespace Assets.FlaImporter.Editor.Data.RawData.FrameElements
{
    public class FlaBaseInstanceRaw : FlaFrameElementRaw
    {
        [XmlAttribute("libraryItemName")]
        public string LibraryItemName;

        [XmlAttribute("selected")]
        public bool Selected;
        
        [XmlAttribute("blendMode")]
        public string BlendMode;

        [XmlAttribute("cacheAsBitmap")]
        public bool CacheAsBitmap;

        [XmlAttribute("referenceID")]
        public string ReferenceId;


        

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
