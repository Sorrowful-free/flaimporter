﻿using System.Xml.Serialization;

namespace Assets.FlaExporter.Data.RawData.Geom
{
    public class FlaMatrixElemetRaw
    {
        [XmlElement("Matrix")] 
        public FlaMatrixRaw Matrix = new FlaMatrixRaw();
    }
}