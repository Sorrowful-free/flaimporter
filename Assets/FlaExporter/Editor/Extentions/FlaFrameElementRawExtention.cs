using Assets.FlaExporter.Editor.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.Utils;
using Assets.FlaExporter.FlaExporter;

namespace Assets.FlaExporter.Editor.Extentions
{
    public static class FlaFrameElementRawExtention
    {
        public static string GetName(this FlaFrameElementRaw element)
        {
            if (element is FlaShapeRaw)
            {
                return ((FlaShapeRaw)element).GetUniqueName();
            }
            if (element is FlaBaseInstanceRaw)
            {
                return ((FlaBaseInstanceRaw)element).LibraryItemName;
            }
            return "";
        }

        public static float GetValueByPropertyType(this FlaFrameElementRaw element, FlaTransformPropertyEnum propertyType)
        {
            switch (propertyType)
            {
                case FlaTransformPropertyEnum.Rotation:
                    return element.Matrix.Matrix.GetAngle();
                case FlaTransformPropertyEnum.PositionX:
                    return element.Matrix.Matrix.GetPosition().x;
                case FlaTransformPropertyEnum.PositionY:
                    return element.Matrix.Matrix.GetPosition().y;
                case FlaTransformPropertyEnum.ScaleX:
                    return element.Matrix.Matrix.GetScale().x;
                case FlaTransformPropertyEnum.ScaleY:
                    return element.Matrix.Matrix.GetScale().y;
                case FlaTransformPropertyEnum.SkewX:
                    return element.Matrix.Matrix.GetSkewX();
                case FlaTransformPropertyEnum.SkewY:
                    return element.Matrix.Matrix.GetSkewY();
                case FlaTransformPropertyEnum.TransformPointX:
                    if (element.CenterPoint3Dx != null && element.CenterPoint3Dx != 0)
                    {
                        return element.CenterPoint3Dx - element.Matrix.Matrix.GetPosition().x;
                    }
                    return element.TransformationPoint.Point.X;
                case FlaTransformPropertyEnum.TransformPointY:
                    if (element.CenterPoint3Dy != null && element.CenterPoint3Dy != 0)
                    {
                        return element.CenterPoint3Dy;
                    }
                    return element.TransformationPoint.Point.Y - element.Matrix.Matrix.GetPosition().y;
                default:
                    return 0;
            }
            return 0;
        }
    }
}
