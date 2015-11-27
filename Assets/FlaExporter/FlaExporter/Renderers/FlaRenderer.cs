using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform;
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

        public void UpdateColorTranform(FlaColorTransform colorTransform)
        {
            foreach (var material in MeshRenderer.sharedMaterials)
            {
                material.SetVector("_ColorMultipler",colorTransform.ColorMultipler);
                material.SetVector("_ColorOffset", colorTransform.ColorOffset);
            }
            
        }

       
        

    }
}
