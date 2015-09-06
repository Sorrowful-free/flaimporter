using System;
using Assets.FlaExporter.Data.RawData.Geom;
using UnityEngine;

namespace Assets.FlaExporter.Editor.Utils
{
    public static class FlaMatrixUtils
    {
       
        public static float GetSkewX(this FlaMatrixRaw flaMatrix)
        {
            return (float)(Math.Atan(-flaMatrix.B/flaMatrix.A)*180/Math.PI);
        }

        public static float GetSkewY(this FlaMatrixRaw flaMatrix)
        {
            return (float)(Math.Atan(flaMatrix.C / flaMatrix.D)*180/Math.PI);
        }

        public static float GetAngle(this FlaMatrixRaw flaMatrix)
        {
            return (flaMatrix.GetSkewX() + flaMatrix.GetSkewY())/2.0f;
        }

        public static Vector2 GetPosition(this FlaMatrixRaw flaMatrix)
        {
            return new Vector2(flaMatrix.TX,flaMatrix.TY);
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
            transform.localScale = flaMatrix.GetScale();
        }
    }
}
