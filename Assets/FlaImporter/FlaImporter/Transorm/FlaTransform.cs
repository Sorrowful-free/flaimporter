using Assets.FlaImporter.FlaImporter.Geom;
using UnityEngine;

namespace Assets.FlaImporter.FlaImporter.Transorm
{
    [ExecuteInEditMode]
    public class FlaTransform : MonoBehaviour
    {
        private static readonly Vector2 ZeroVec2d = Vector2.zero;
        private static readonly Vector3 ForwardVec  = Vector3.forward;

        [SerializeField]
        private Vector2 _transformPoint;

        [SerializeField]
        private FlaMatrix2D _matrix2D;

        private void LateUpdate()
        {
            if (_matrix2D.UpdateMatrix())
            {
                var scale2d = _matrix2D.GetScale();
                var scale3d = transform.localScale;
                scale3d.x = scale2d.x;
                scale3d.y = scale2d.y;
                transform.localScale = scale3d;

                var position2d = _matrix2D.GetPosition();
                var position3d = transform.localPosition;
                position3d.x = position2d.x;
                position3d.y = position2d.y;
                transform.localPosition = position3d;

                var rotation2d = _matrix2D.GetAngle();
                var rotation3d = transform.localEulerAngles;
                rotation3d.z = rotation2d;
                
                if (_transformPoint != ZeroVec2d)
                {
                    var deltaAngle = rotation2d - rotation3d.z;
                    var localToGlobal = transform.TransformPoint(_transformPoint);
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
