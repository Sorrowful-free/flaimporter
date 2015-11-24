using System;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderers.FillStyles
{
    [Serializable]
    public class BaseFillStyle
    {
        public Material Material { get; private set; }

        public BaseFillStyle(Material material)
        {
            Material = material;
        }

        public virtual void DrawGUI()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(this.GetType().Name);
            EditorGUILayout.ObjectField("Material", Material,typeof(Material));

        }
    }
}
