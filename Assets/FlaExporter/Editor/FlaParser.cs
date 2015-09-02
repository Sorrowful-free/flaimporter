using Assets.FlaExporter.Data.RawData;
using Assets.Scripts.Helpers.Extensions;

namespace Assets.FlaExporter.Editor
{
    public static class FlaParser
    {
        public static FlaSymbolItemRaw ProcessSymbolXml(byte[] bytes)
        {
            var fla = bytes.ObjectFromXML<FlaSymbolItemRaw>();
            return fla;
        }

        public static FlaDocumentRaw ProcessDocumentXml(byte[] bytes)
        {
            var fla = bytes.ObjectFromXML<FlaDocumentRaw>(); 
            return fla;
        }
    }
}
