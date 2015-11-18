
using System.Collections.Generic;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter
{
    [ExecuteInEditMode]
    public class FlaTransform : MonoBehaviour
    {
        public readonly static Dictionary<FlaTransformPropertyEnum, string> ProperyNames = new Dictionary<FlaTransformPropertyEnum, string>
        {
            { FlaTransformPropertyEnum.Rotation, "Rotation"},
            { FlaTransformPropertyEnum.PositionX, "Position.x"},
            { FlaTransformPropertyEnum.PositionY, "Position.y"},
            { FlaTransformPropertyEnum.ScaleX, "Scale.x"},
            { FlaTransformPropertyEnum.ScaleY, "Scale.y"},
            { FlaTransformPropertyEnum.SkewX, "Skew.x"},
            { FlaTransformPropertyEnum.SkewY, "Skew.y"},
            { FlaTransformPropertyEnum.TransformPointX, "TransformPoint.x"},
            { FlaTransformPropertyEnum.TransformPointY, "TransformPoint.y"},
        };

        [HideInInspector]
        [SerializeField]
        public float Rotation = 0;
        private float _oldRotation = 0;

        [HideInInspector]
        [SerializeField]
        public Vector2 Position = Vector2.zero;
        private Vector2 _oldPosition = Vector2.zero;

        [HideInInspector]
        [SerializeField]
        public  Vector2 Scale = Vector2.one;
        private Vector2 _oldScale = Vector2.one;

        [HideInInspector]
        [SerializeField]
        public Vector2 Skew = Vector2.zero;
        private Vector2 _oldSkew = Vector2.zero;

        [HideInInspector]
        [SerializeField]
        public Vector2 TransformPoint = Vector2.zero;
        private Vector2 _oldTransformPoint = Vector2.zero;
        
        private void Update()
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
                if (TransformPoint != Vector2.zero)
                {
                    var deltaAngle = Rotation - transform.eulerAngles.z;
                    var localToGlobal = transform.TransformPoint(TransformPoint);
                    transform.RotateAround(localToGlobal, Vector3.forward, deltaAngle);
                    Position = _oldPosition = transform.position;
                }
                else
                {
                    transform.eulerAngles = Vector3.forward * Rotation;
                }
                _oldRotation = Rotation;
            }
        }

       
    }

    public enum FlaTransformPropertyEnum
    {
        Rotation,
        PositionX,
        PositionY,
        ScaleX,
        ScaleY,
        SkewX,
        SkewY,
        TransformPointX,
        TransformPointY,
    }
}
