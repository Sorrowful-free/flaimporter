﻿using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.FillStyles.FillStyles
{
    public class FlaSolidColorFillStyleRaw : FlaBaseFillStyleRaw
    {
        [XmlAttribute("color")] 
        public string Color;
    }
}