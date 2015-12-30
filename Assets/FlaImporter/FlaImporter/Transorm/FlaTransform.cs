using UnityEngine;

namespace Assets.FlaImporter.FlaImporter.Transorm
{
    [ExecuteInEditMode]
    public class FlaTransform : MonoBehaviour
    {
        private static readonly Vector2 ZeroVec2d = Vector2.zero;
        private static readonly Vector3 ForwardVec = Vector3.forward;
        private static Matrix4x4 _matrix = Matrix4x4.identity;

        [SerializeField] public float Rotation = 0;
        private float _oldRotation;

        [SerializeField]
        public float Order = 0;
        private float _oldOrder;

        [SerializeField] public Vector2 Position = Vector2.zero;
        private Vector2 _oldPosition = Vector2.zero;

        [SerializeField] public Vector2 Scale = Vector2.one;
        private Vector2 _oldScale = Vector2.one;

        [SerializeField] public Vector2 Skew = Vector2.zero;
        private Vector2 _oldSkew = Vector2.zero;

        [SerializeField] public Vector2 TransformPoint = Vector2.zero;
        private Vector2 _oldTransformPoint = Vector2.zero;

        private void OnEnable()
        {
            LateUpdate();
        }

        private void LateUpdate()
        {
            
            if (_oldPosition != Position)
            {
                var oldPosition = transform.localPosition;
                oldPosition.x = Position.x;
                oldPosition.y = Position.y;
                transform.localPosition = oldPosition;
                _oldPosition = Position;
            }

            if (_oldRotation != Rotation || TransformPoint != _oldTransformPoint)
            {
                if (TransformPoint != ZeroVec2d)
                {
                    _matrix = Matrix4x4.identity; // kostil and memory leak

                    var tempScale = transform.localScale;
                    _matrix.SetTRS(-TransformPoint, Quaternion.Euler(ForwardVec * Rotation), tempScale);
                    //var deltaAngle = Rotation - transform.eulerAngles.z;
                    //var localToGlobal = transform.TransformPoint(TransformPoint);
                    //transform.RotateAround(localToGlobal, ForwardVec, deltaAngle);
                    tempScale.x = 1 / tempScale.x;
                    tempScale.y = 1 / tempScale.y;
                    tempScale.z = 1 / tempScale.z;

                    _matrix.SetTRS(TransformPoint, Quaternion.identity, tempScale);
                    
                    Position = _oldPosition =  _matrix*Position;
                    var tempPosition = transform.position;
                    tempPosition.x = Position.x;
                    tempPosition.y = Position.y;
                    //Position = _oldPosition = transform.position;
                    _oldTransformPoint = TransformPoint;
                }
                transform.eulerAngles = ForwardVec * Rotation;
                _oldRotation = Rotation;
            }

            if (_oldScale != Scale)
            {
                var scale = transform.localScale;
                scale.x = Scale.x;
                scale.y = Scale.y;
                transform.localScale = scale;
                _oldScale = Scale;
            }
            
            if (Order != _oldOrder)
            {
                var oldPosition = transform.localPosition;
                _oldOrder = oldPosition.z = Order;
                transform.localPosition = oldPosition;
                _oldPosition = Position;
            }
               
        }
    }
}
