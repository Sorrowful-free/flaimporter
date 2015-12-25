using System;
using Assets.FlaImporter.Editor.Data.RawData.Geom;
using UnityEngine;

namespace Assets.FlaImporter.Editor.Utils
{
    public static class FlaMatrixRawExtention
    {
        public static float GetSkewX(this FlaMatrixRaw flaMatrix)
        {
            return (float)(Math.Atan(-flaMatrix.B/flaMatrix.A)*180/Math.PI);// -a for convert angles to unity
        }

        public static float GetSkewY(this FlaMatrixRaw flaMatrix)
        {
            return (float)(Math.Atan(flaMatrix.C / flaMatrix.D)*180/Math.PI); //-d for convert angles to unity
        }

        public static float GetAngle(this FlaMatrixRaw flaMatrix)
        {
            return (flaMatrix.GetSkewX() + flaMatrix.GetSkewY())/2.0f;
        }
        
        public static Vector2 GetPosition(this FlaMatrixRaw flaMatrix)
        {
            return new Vector2(flaMatrix.TX,-flaMatrix.TY)/FlaImporterConstatns.PixelsPerUnits;//-ty for convert to unity
        }

        public static Vector2 GetScale(this FlaMatrixRaw flaMatrix)
        {
            var sx = (float)Math.Sqrt(Math.Pow(flaMatrix.A, 2) + Math.Pow(flaMatrix.C, 2));
            var sy = (float)Math.Sqrt(Math.Pow(flaMatrix.B, 2) + Math.Pow(flaMatrix.D, 2));
            return new Vector2(sx,sy);
        }

        public static void CopyMatrix(this FlaMatrixRaw flaMatrix, Transform transform)
        {
            transform.eulerAngles = Vector3.forward*flaMatrix.GetAngle();
            transform.position = flaMatrix.GetPosition();
            var scale = (Vector3) flaMatrix.GetScale();
            scale.z = 1;
            transform.localScale = scale;
        }

        public static Vector4 GetABCD(this FlaMatrixRaw flaMatrix)
        {
            return new Vector4(flaMatrix.A, flaMatrix.B, flaMatrix.C, flaMatrix.D);
        }

        public static Vector2 GetTXTY(this FlaMatrixRaw flaMatrix)
        {
            return new Vector2(flaMatrix.TX, flaMatrix.TY);
        }
    }
}
