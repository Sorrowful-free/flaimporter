using Assets.FlaExporter.FlaExporter.Renderer.FillStyles;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderer
{
    [ExecuteInEditMode]
    public class FlaEdge : MonoBehaviour
    {
        public FlaFillStyle FillStyle;
        

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
