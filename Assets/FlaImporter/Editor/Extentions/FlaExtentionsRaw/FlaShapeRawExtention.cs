using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;

namespace Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw
{
    public static class FlaShapeRawExtention
    {
        public static string GetUniqueName(this FlaShapeRaw shape)
        {
            return "shape" + shape.Edges.Select(e => e.Edges).JoinToString("->").GetHashCode();
        }
    }
}
