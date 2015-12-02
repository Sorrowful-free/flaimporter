using Assets.FlaExporter.FlaExporter.Renderer.FillStyles;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderer
{
    [ExecuteInEditMode]
    public class FlaEdge : MonoBehaviour
    {
        public FlaShape Parent;
        public FlaFillStyle FillStyle;

        private MeshRenderer _meshRenderer;
        public MeshRenderer MeshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                {
                    _meshRenderer = GetComponent<UnityEngine.MeshRenderer>();
                }
                return _meshRenderer;
            }
        }
        private void LateUpdate()
        {
            FillStyle.UpdateMaterial();
        }

        private void OnEnable()
        {
             FillStyle.UpdateMaterialWithoutCheck();
        }
    }
}
