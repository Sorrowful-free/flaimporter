using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.FlaTransorm.Enums;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.FlaTransorm
{
    [ExecuteInEditMode]
    public class FlaTransform : MonoBehaviour
    {
        public static readonly Vector2 ZeroVec = Vector2.zero;
        public static readonly Vector3 ForwardVec = Vector3.forward;

        public readonly static Dictionary<FlaTransformPropertyTypeEnum, string> ProperyNames = new Dictionary<FlaTransformPropertyTypeEnum, string>
        {
            { FlaTransformPropertyTypeEnum.ScaleX, "Scale.x"},
            { FlaTransformPropertyTypeEnum.ScaleY, "Scale.y"},
            { FlaTransformPropertyTypeEnum.PositionX, "Position.x"},
            { FlaTransformPropertyTypeEnum.PositionY, "Position.y"},
            { FlaTransformPropertyTypeEnum.TransformPointX, "TransformPoint.x"},
            { FlaTransformPropertyTypeEnum.TransformPointY, "TransformPoint.y"},
            { FlaTransformPropertyTypeEnum.Rotation, "Rotation"},
            { FlaTransformPropertyTypeEnum.SkewX, "Skew.x"},
            { FlaTransformPropertyTypeEnum.SkewY, "Skew.y"},
           
        };

        public readonly static Dictionary<FlaTransformPropertyTypeEnum, float> ProperyDefaultValues = new Dictionary<FlaTransformPropertyTypeEnum, float>
        {
            { FlaTransformPropertyTypeEnum.Rotation, 0},
            { FlaTransformPropertyTypeEnum.PositionX, 0},
            { FlaTransformPropertyTypeEnum.PositionY, 0},
            { FlaTransformPropertyTypeEnum.ScaleX, 1},
            { FlaTransformPropertyTypeEnum.ScaleY, 1},
            { FlaTransformPropertyTypeEnum.SkewX, 0},
            { FlaTransformPropertyTypeEnum.SkewY, 0},
            { FlaTransformPropertyTypeEnum.TransformPointX, 0},
            { FlaTransformPropertyTypeEnum.TransformPointY, 0},
        };

        [SerializeField]
        public float Rotation = 0;
        private float _oldRotation = 0;

        [SerializeField]
        public Vector2 Position = Vector2.zero;
        private Vector2 _oldPosition = Vector2.zero;

        [SerializeField]
        public  Vector2 Scale = Vector2.one;
        private Vector2 _oldScale = Vector2.one;

        [SerializeField]
        public Vector2 Skew = Vector2.zero;
        private Vector2 _oldSkew = Vector2.zero;

        [SerializeField]
        public Vector2 TransformPoint = Vector2.zero;
        private Vector2 _oldTransformPoint = Vector2.zero;

#if UNITY_EDITOR
         private void Update()
#elif
         private void LateUpdate()
#endif
        {
            if (_oldScale != Scale)
            {
                var scale = (Vector3)Scale;
                scale.z = 1;
                transform.localScale = scale;
                _oldScale = Scale;
            }

            if (_oldPosition != Position)
            {
                transform.localPosition = Position;
                _oldPosition = Position;
            }

            if (_oldRotation != Rotation)
            {
                if (TransformPoint != ZeroVec)
                {
                    var deltaAngle = Rotation - transform.eulerAngles.z;
                    var localToGlobal = transform.TransformPoint(TransformPoint);
                    transform.RotateAround(localToGlobal, ForwardVec, deltaAngle);
                    Position = _oldPosition = transform.position;
                }
                else
                {
                    transform.eulerAngles = ForwardVec * Rotation;
                }
                _oldRotation = Rotation;
            }
        }
    }
}
