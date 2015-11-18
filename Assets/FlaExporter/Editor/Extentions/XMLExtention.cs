using System.IO;
using System.Xml.Serialization;

namespace Assets.FlaExporter.Editor.Extentions
{
    public static class XMLExtention
    {
        public static TObject ObjectFromXML<TObject>(this byte[] data) where TObject : class
        {
            var serializer = new XmlSerializer(typeof(TObject));
            var container = serializer.Deserialize(new MemoryStream(data)) as TObject;
            return container;
        }

        public static byte[] XMLFromObject<TObject>(this TObject obj) where TObject : class
        {
            var serializer = new XmlSerializer(typeof(TObject));
            var memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream,obj);
            return memoryStream.GetBuffer();
        }

    }
}
