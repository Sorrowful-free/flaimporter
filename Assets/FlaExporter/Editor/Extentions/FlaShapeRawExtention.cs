using System.Linq;
using Assets.BundleExporter.Editor.Helpers;
using Assets.FlaExporter.Data.RawData.FrameElements;

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
