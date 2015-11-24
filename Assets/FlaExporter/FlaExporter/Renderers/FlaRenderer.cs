using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.Renderers.FillStyles;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderers
{
    [ExecuteInEditMode]
    public class FlaRenderer : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        private MeshRenderer MeshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                {
                    _meshRenderer = GetComponent<MeshRenderer>();
                }
                return _meshRenderer;
            }
        }

        public bool Visible
        {
            get { return MeshRenderer.enabled; }
            set { MeshRenderer.enabled = value; }
        }

        [HideInInspector]
        public List<BaseFillStyle> FillStyles = new List<BaseFillStyle>()
        {
            new SolidColorFillStyle(new Material("asd"){name = "test"}),
            new SolidColorFillStyle(new Material("asd"){name = "test"}),
            new SolidColorFillStyle(new Material("asd"){name = "test"}),
        };
        

    }
}
