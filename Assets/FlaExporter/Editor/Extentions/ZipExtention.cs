using System.IO;
using Ionic.Zip;

namespace Assets.FlaExporter.Editor.Extentions
{
    public static class ZipExtention
    {
        public static byte[] ToByteArray(this ZipEntry entry)
        {
            var writeSpace = new MemoryStream();
            writeSpace.Position = 0;

            var reader = entry.OpenReader();
            var buffer = new byte[2048];
            var len = 0;
            while ((len = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                writeSpace.Write(buffer, 0, len);
            }
            return writeSpace.ToArray();
        }
    }
}
