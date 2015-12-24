using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform.Enums;
using Assets.FlaExporter.FlaExporter.Transorm.Enums;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.Utils;

namespace Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw
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

        public static float GetColorValueByPropertyType(this FlaFrameElementRaw element,FlaColorTransformPropertyTypeEnum propertyType)
        {
            switch (propertyType)
            {
                case FlaColorTransformPropertyTypeEnum.ColorMultiplyR:
                    return element.Color.Color.RedMultiplier;
                    
                case FlaColorTransformPropertyTypeEnum.ColorMultiplyG:
                    return element.Color.Color.GreenMultiplier;

                case FlaColorTransformPropertyTypeEnum.ColorMultiplyB:
                    return element.Color.Color.BlueMultiplier;

                case FlaColorTransformPropertyTypeEnum.ColorMultiplyA:
                    return element.Color.Color.AlphaMultiplier;
                    
                case FlaColorTransformPropertyTypeEnum.ColorOffsetR:
                    return (float)element.Color.Color.RedOffset/255.0f;
                    
                case FlaColorTransformPropertyTypeEnum.ColorOffsetG:
                    return (float)element.Color.Color.GreenOffset / 255.0f;
                    
                case FlaColorTransformPropertyTypeEnum.ColorOffsetB:
                    return (float)element.Color.Color.BlueOffset / 255.0f;

                case FlaColorTransformPropertyTypeEnum.ColorOffsetA:
                    return (float)element.Color.Color.AlphaOffset / 255.0f;

                default:
                    return 0;
            }
        }

        public static float GetTransformValueByPropertyType(this FlaFrameElementRaw element, FlaTransformPropertyTypeEnum propertyType)
        {
            switch (propertyType)
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
                        return element.CenterPoint3Dx/FlaImporterConstatns.PixelsPerUnits - element.Matrix.Matrix.GetPosition().x;
                    }
                    return element.TransformationPoint.Point.X/FlaImporterConstatns.PixelsPerUnits;

                case FlaTransformPropertyTypeEnum.TransformPointY:
                    if (element.CenterPoint3Dy != null && element.CenterPoint3Dy != 0)
                    {
                        return (-element.CenterPoint3Dy/FlaImporterConstatns.PixelsPerUnits) - element.Matrix.Matrix.GetPosition().y;
                        ;
                    }
                    return element.TransformationPoint.Point.Y/FlaImporterConstatns.PixelsPerUnits;
                default:
                    return 0;
            }
            return 0;
        }
    }
}
