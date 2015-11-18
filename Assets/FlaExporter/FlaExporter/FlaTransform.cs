using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter
{
    public class FlaTransform : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private float _rotation = 0;
        private float _oldRotation = 0;

        [HideInInspector]
        [SerializeField]
        private Vector2 _position = Vector2.zero;
        private Vector2 _oldPosition = Vector2.zero;

        [HideInInspector]
        [SerializeField]
        private Vector2 _scale = Vector2.one;
        private Vector2 _oldScale = Vector2.one;

        [HideInInspector]
        [SerializeField]
        private Vector2 _skew = Vector2.zero;
        private Vector2 _oldSkew = Vector2.zero;

        [HideInInspector]
        [SerializeField]
        private Vector2 _transformPoint = Vector2.zero;
        private Vector2 _oldTransformPoint = Vector2.zero;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnValidate();
            }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnValidate();
            }
        }

        public Vector2 Skew
        {
            get { return _skew; }
            set
            {
                _skew = value;
                OnValidate();
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                OnValidate();
            }
        }

        public Vector2 TransformPoint
        {
            get { return _transformPoint; }
            set
            {
                _transformPoint = value;
                OnValidate();
            }
        }

        private void LateUpdate()
        {
            if (_oldScale != _scale)
            {
                var scale = (Vector3)_scale;
                scale.z = 1;
                transform.localScale = scale;
                _oldScale = _scale;
            }

            if (_oldPosition != _position)
            {
                transform.localPosition = _position;
                _oldPosition = _position;
            }

            if (_oldRotation != _rotation)
            {
                if (_transformPoint != Vector2.zero)
                {
                    var deltaAngle = _rotation - transform.eulerAngles.z;
                    var localToGlobal = transform.TransformPoint(_transformPoint);
                    transform.RotateAround(localToGlobal, Vector3.forward, deltaAngle);
                    _position = _oldPosition = transform.position;
                }
                else
                {
                    transform.eulerAngles = Vector3.forward * _rotation;
                }
                _oldRotation = _rotation;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            LateUpdate();
        }
#endif
    }
}
