using System;
using Assets.FlaImporter.FlaImporter.Geom;
using UnityEngine;

namespace Assets.FlaImporter.FlaImporter.Renderer.FillStyles
{
    [Serializable]
    public struct FlaFillStyle
    {
        [SerializeField]
        public FlaMatrix2D Matrix;

        [HideInInspector]
        [SerializeField]
        private MeshRenderer _meshRenderer;
        public Material Material 
        {
            get
            {
                if(_meshRenderer != null)
                    return !Application.isPlaying ? _meshRenderer.sharedMaterial:_meshRenderer.material;
                return null;
            }
        }

    //    [HideInInspector]
        [SerializeField]
        private float _aspect;
        private float _oldAspect; 

      //  [HideInInspector]
        [SerializeField]
        private bool _isCliped;
        private bool _oldIsCliped;


        public FlaFillStyle(MeshRenderer meshRenderer, FlaMatrix2D matrix, float aspect, bool isCliped)
        {
            _oldAspect = _aspect = aspect;
            Matrix = matrix;
            _meshRenderer = meshRenderer;
            _oldIsCliped = _isCliped = isCliped;
        }
        
        public void UpdateMaterial()
        {
            if (Matrix.UpdateMatrix() || _isCliped != _oldIsCliped || _aspect != _oldAspect)
            {
                _oldIsCliped = _isCliped;
                _oldAspect = _aspect;
                UpdateMaterialWithoutCheck();
            }
        }

        public void UpdateMaterialWithoutCheck()
        {
            if (_meshRenderer == null)
            {
                return;
            }
            if (Material == null) 
                return;
            Material.SetVector("_TextureMatrixABCD", Matrix.ABCD);
            Material.SetVector("_TextureMatrixTXTY", Matrix.TXTY);
            Material.SetFloat("_TextureAspect", _aspect); 
            Material.SetInt("_TextureIsCliped", _isCliped ? 1 : 0);
        }

    }
}
