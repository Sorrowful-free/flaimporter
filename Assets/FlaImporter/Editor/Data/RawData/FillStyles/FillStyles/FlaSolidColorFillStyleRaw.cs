using System.Xml.Serialization;

namespace Assets.FlaImporter.Editor.Data.RawData.FillStyles.FillStyles
{
    public class FlaSolidColorFillStyleRaw : FlaBaseFillStyleRaw
    {
        [XmlAttribute("color")] 
        public string Color = "#000000";
    }
}