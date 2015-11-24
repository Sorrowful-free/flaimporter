using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderers.FillStyles
{
    public class SolidColorFillStyle : BaseFillStyle
    {
        private Color _color;

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value; 
                Material.SetColor("_Color",_color);
            }
        }

        public SolidColorFillStyle(Material material) : base(material)
        {
        }

        public override void DrawGUI()
        {
            base.DrawGUI();
            Color = EditorGUILayout.ColorField("Color", Color);
        }
    }
}
