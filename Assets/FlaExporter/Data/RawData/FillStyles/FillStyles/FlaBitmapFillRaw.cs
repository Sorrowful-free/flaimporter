using System.Xml.Serialization;
using Assets.FlaExporter.Data.RawData.Geom;

namespace Assets.FlaExporter.Data.RawData.FillStyles.FillStyles
{
    public class FlaBitmapFillRaw : FlaBaseFillStyleRaw
    {
        [XmlAttribute("bitmapPath")]
        public string BitmapPath;
    }
}
