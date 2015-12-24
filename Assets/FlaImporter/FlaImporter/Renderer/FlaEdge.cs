using Assets.FlaImporter.FlaImporter.Renderer.FillStyles;
using UnityEngine;

namespace Assets.FlaImporter.FlaImporter.Renderer
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
