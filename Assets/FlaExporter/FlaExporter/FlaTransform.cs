using UnityEngine;

namespace Assets.FlaExporter.FlaExporter
{
    public class FlaTransform : MonoBehaviour
    {
        
        public float Rotation 
        {
            get { return transform.eulerAngles.z; }
            set
            {
                if (TransformPoint != Vector2.zero)
                {
                    var deltaAngle = value - transform.eulerAngles.z;
                    var localToGlobal = transform.TransformPoint(TransformPoint);
                    transform.RotateAround(localToGlobal, Vector3.forward, deltaAngle);
                    return;
                }
                transform.eulerAngles = Vector3.forward*value;
            }
        }
        public Vector2 Position 
        {
            get{ return transform.localPosition; }
            set
            {
                transform.localPosition = value;
            }
        }
        
        public Vector2 Scale 
        {
            get { return transform.localScale; }
            set
            {
                var scale = (Vector3) value;
                scale.z = 1;
                transform.localScale = scale;
            }
        }
        public Vector2 Skew { get; set; }
        public Vector2 TransformPoint{ get; set; }

    }
}
