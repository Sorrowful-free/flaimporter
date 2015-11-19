using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderers
{
    
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


    }
}
