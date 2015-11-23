﻿using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.FillStyles.FillStyles.GradientFillStyles
{
    public class FlaGradientEntryRaw
    {
        [XmlAttribute("color")] 
        public string Color ="#000000";

        [XmlAttribute("alpha")]
        public float Alpha = 1;
        
        [XmlAttribute("ratio")]
        public float Ratio;
    }
}