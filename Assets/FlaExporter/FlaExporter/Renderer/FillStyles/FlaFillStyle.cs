using System;
using Assets.FlaExporter.FlaExporter.Geom;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderer.FillStyles
{
    [Serializable]
    public struct FlaFillStyle
    {
        [SerializeField]
        public Material Material;
        [SerializeField]
        public float Aspect;
        [SerializeField]
        public FlaMatrix2D Matrix;

        public FlaFillStyle(Material material,float aspect,FlaMatrix2D matrix)
        {
            Material = material;
            Aspect = aspect;
            Matrix = matrix;
        }
        
        public void UpdateMaterial()
        {
            if (Matrix.UpdateMatrix())
            {
                Material.SetVector("_TextureMatrixABCD", Matrix.ABCD);
                Material.SetVector("_TextureMatrixTXTY", Matrix.TXTY);
                Material.SetFloat("_TextureAspect", Aspect);
            }
        }


    }
}
