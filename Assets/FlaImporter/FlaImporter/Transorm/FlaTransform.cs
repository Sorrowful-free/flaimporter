using Assets.FlaImporter.FlaImporter.Geom;
using UnityEngine;

namespace Assets.FlaImporter.FlaImporter.Transorm
{
    [ExecuteInEditMode]
    public class FlaTransform : MonoBehaviour
    {
        private static readonly Vector2 ZeroVec2d = Vector2.zero;
        private static readonly Vector3 ForwardVec  = Vector3.forward;

        
        public Vector2 TransformPoint;

        public FlaMatrix2D Matrix2D = new FlaMatrix2D(new Vector4(1,0,0,1),Vector2.zero);
        private float _lastRotation;

        private void LateUpdate()
        {
            if (Matrix2D.UpdateMatrix())
            {
                var scale2d = Matrix2D.GetScale();
                var scale3d = transform.localScale;
                scale3d.x = scale2d.x;
                scale3d.y = scale2d.y;
                transform.localScale = scale3d;

                var position2d = Matrix2D.GetPosition();
                var position3d = transform.localPosition;
                position3d.x = position2d.x;
                position3d.y = position2d.y;
                transform.localPosition = position3d;

                var rotation2d = Matrix2D.GetAngle();
                var rotation3d = transform.localEulerAngles;
                rotation3d.z = rotation2d;
                
                if (TransformPoint != ZeroVec2d)
                {
                    var deltaAngle = rotation2d - transform.localEulerAngles.z;
                    Debug.Log("deltaAngle rot" +deltaAngle);
                    var localToGlobal = transform.TransformPoint(TransformPoint);
                    transform.RotateAround(localToGlobal, ForwardVec, deltaAngle);
                }
                else
                {
                    transform.localEulerAngles = rotation3d;
                }
            }
        }
    }
}
