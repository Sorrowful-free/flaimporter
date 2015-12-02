using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderer
{
    [ExecuteInEditMode]
    public class FlaShape: MonoBehaviour
    {
       
      

        [SerializeField]
        public List<FlaEdge> Edges = new List<FlaEdge>();
        public void UpdateColorTranform(FlaColorTransform colorTransform)
        {
           
            foreach (var flaEdge in Edges)
            {
                var material = flaEdge.FillStyle.Material;
                material.SetColor("_ColorMultipler", colorTransform.ColorMultipler);
                material.SetVector("_ColorOffset", colorTransform.ColorOffset);
            }
        }
    }
}
