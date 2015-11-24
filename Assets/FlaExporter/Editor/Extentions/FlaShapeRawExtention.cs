using System.Linq;
using Assets.FlaExporter.Editor.Data.RawData.FrameElements;

namespace Assets.FlaExporter.Editor.Extentions
{
    public static class FlaShapeRawExtention
    {
        public static string GetUniqueName(this FlaShapeRaw shape)
        {
            return "shape" + shape.Edges.Select(e => e.Edges).JoinToString("->").GetHashCode();
        }
    }
}
