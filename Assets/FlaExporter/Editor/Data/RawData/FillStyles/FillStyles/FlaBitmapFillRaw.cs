using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Data.RawData.FillStyles.FillStyles
{
    public class FlaBitmapFillRaw : FlaBaseFillStyleRaw
    {
        [XmlAttribute("bitmapPath")]
        public string BitmapPath;
    }
}
