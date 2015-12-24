using System;
using Assets.FlaExporter.FlaExporter.Geom;
using Assets.FlaExporter.FlaExporter.Renderer.Enums;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderer.FillStyles
{
    [Serializable]
    public struct FlaFillStyle
    {


        [SerializeField]
        public FlaMatrix2D Matrix;

        [HideInInspector]
        [SerializeField]
        private Material _material;
        public Material Material
        {
            get { return _material; }
        }

    //    [HideInInspector]
        [SerializeField]
        private float _aspect;

      //  [HideInInspector]
        [SerializeField]
        private bool _isCliped;


        public FlaFillStyle(Material material, FlaMatrix2D matrix, float aspect, bool isCliped)
        {
            _aspect = aspect;
            Matrix = matrix;
            _material = material;
            _isCliped = isCliped;
        }
        
        public void UpdateMaterial()
        {
            if (Matrix.UpdateMatrix())
            {
                UpdateMaterialWithoutCheck();
            }
        }

        public void UpdateMaterialWithoutCheck()
        {
            if (_material == null)
                return;
            _material.SetVector("_TextureMatrixABCD", Matrix.ABCD);
            _material.SetVector("_TextureMatrixTXTY", Matrix.TXTY);
            _material.SetFloat("_TextureAspect", _aspect);
            _material.SetFloat("_TextureIsCliped", _isCliped?1:0);
            
        }

    }
}
