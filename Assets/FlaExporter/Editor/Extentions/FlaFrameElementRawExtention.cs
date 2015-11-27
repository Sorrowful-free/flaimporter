using Assets.FlaExporter.Editor.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.Utils;
using Assets.FlaExporter.FlaExporter;
using Assets.FlaExporter.FlaExporter.Transorm.Enums;

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

        public static float GetValueByPropertyType(this FlaFrameElementRaw element, FlaTransformPropertyTypeEnum propertyTypeType)
        {
            switch (propertyTypeType)
            {
                case FlaTransformPropertyTypeEnum.Rotation:
                    return element.Matrix.Matrix.GetAngle();
                case FlaTransformPropertyTypeEnum.PositionX:
                    return element.Matrix.Matrix.GetPosition().x;
                case FlaTransformPropertyTypeEnum.PositionY:
                    return element.Matrix.Matrix.GetPosition().y;
                case FlaTransformPropertyTypeEnum.ScaleX:
                    return element.Matrix.Matrix.GetScale().x;
                case FlaTransformPropertyTypeEnum.ScaleY:
                    return element.Matrix.Matrix.GetScale().y;
                case FlaTransformPropertyTypeEnum.SkewX:
                    return element.Matrix.Matrix.GetSkewX();
                case FlaTransformPropertyTypeEnum.SkewY:
                    return element.Matrix.Matrix.GetSkewY();
                case FlaTransformPropertyTypeEnum.TransformPointX:
                    if (element.CenterPoint3Dx != null && element.CenterPoint3Dx != 0)
                    {
                        return element.CenterPoint3Dx / FlaExporterConstatns.PixelsPerUnits - element.Matrix.Matrix.GetPosition().x;
                    }
                    return element.TransformationPoint.Point.X / FlaExporterConstatns.PixelsPerUnits;

                case FlaTransformPropertyTypeEnum.TransformPointY:
                    if (element.CenterPoint3Dy != null && element.CenterPoint3Dy != 0)
                    {
                        return (-element.CenterPoint3Dy /FlaExporterConstatns.PixelsPerUnits) - element.Matrix.Matrix.GetPosition().y;;
                    }
                    return element.TransformationPoint.Point.Y/FlaExporterConstatns.PixelsPerUnits;
                default:
                    return 0;
            }
            return 0;
        }
    }
}
