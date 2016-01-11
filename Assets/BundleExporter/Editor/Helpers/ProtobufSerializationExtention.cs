using System.IO;
using ProtoBuf;

namespace Assets.BundleExporter.Editor.Helpers
{
    public static class ProtobufSerializationExtention
    {
        internal static byte[] Serialize<TObject>(this TObject @object)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, @object);
            return stream.ToArray(); 
        }

        internal static TObject Deserialize<TObject>(this byte[] data)
        {
            return Serializer.Deserialize<TObject>(new MemoryStream(data));
        }
    }

}
