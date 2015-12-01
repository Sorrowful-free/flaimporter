using Assets.FlaExporter.FlaExporter.Renderer;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter
{
    public class TestShader : MonoBehaviour
    {
        [Range(0,360)]
        public float Rotation;
        private FlaEdge _renderer;

        public FlaEdge Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = (FlaEdge)GetComponent(typeof(FlaEdge));
                }
                return _renderer;
            }
        }

        private void OnValidate()
        {
            // a = cos 
            //b = - sin
            //c = sin
            //d = cos
            var sin = Mathf.Sin(Mathf.PI/180*Rotation)*20;
            var cos = Mathf.Cos(Mathf.PI/180*Rotation)*20;
            var style = Renderer.FillStyle;
            style.Matrix.ABCD = new Vector4(cos, -sin, sin, cos);
            Renderer.FillStyle = style;
            //for (int i = 0; i < Renderer.FillStyles.Count; i++)
            //{
            //    var style = Renderer.FillStyles[i]; 
            //    style.Matrix.ABCD = new Vector4(cos, -sin, sin, cos);
            //    Renderer.FillStyles[i] = style;
            //}
        }
    }
}
