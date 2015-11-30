using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform;
using Assets.FlaExporter.FlaExporter.Renderer.FillStyles;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderer
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

        public List<FlaFillStyle> FillStyles = new List<FlaFillStyle>();

        public void UpdateColorTranform(FlaColorTransform colorTransform)
        {
            foreach (var material in MeshRenderer.sharedMaterials)
            {
                material.SetColor("_ColorMultipler",colorTransform.ColorMultipler);
                material.SetVector("_ColorOffset", colorTransform.ColorOffset);
            }
        }

#if UNITY_EDITOR
        private void Update()
#elif
        private void LateUpdate()
#endif
        {
            
            if (FillStyles != null)
            {
                foreach (var fillStyle in FillStyles)
                {
                    fillStyle.UpdateMaterial();
                }
            }
        }

        private void OnEnable()
        {
            
        }
    }
}
