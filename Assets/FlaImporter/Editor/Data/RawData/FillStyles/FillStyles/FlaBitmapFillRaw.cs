﻿using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.FillStyles.FillStyles
{
    public class FlaBitmapFillRaw : FlaBaseFillStyleRaw
    {
        [XmlAttribute("bitmapPath")]
        public string BitmapPath;

        [XmlAttribute("bitmapIsClipped")]
        public bool BitmapIsClipped;
    }
}
