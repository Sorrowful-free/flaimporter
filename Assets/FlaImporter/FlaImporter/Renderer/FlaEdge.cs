using Assets.FlaImporter.FlaImporter.Renderer.FillStyles;
using Assets.FlaImporter.FlaImporter.Transorm;
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

       
        public Vector2 Skew
        {
            get
            {
                return GetSkew(transform);
            }
        }

        private Vector2 GetSkew(Transform transform)
        {
            var skew = Vector2.zero;
            var flaTransform = transform.GetComponent<FlaTransform>();
            if (flaTransform != null)
            {
                skew = flaTransform.Skew;
            }

            if (transform.parent != null)
            {
                var parentSkew = GetSkew(transform.parent);
                skew += parentSkew;
            }
            return skew;
        }


        private void LateUpdate()
        {
            FillStyle.UpdateMaterial();
            if (FillStyle.Material != null)
                FillStyle.Material.SetVector("_Skew",Skew);
        }

        private void OnEnable()
        {
             FillStyle.UpdateMaterialWithoutCheck();
             if (FillStyle.Material != null)
                FillStyle.Material.SetVector("_Skew", Skew);
        }
    }
}
