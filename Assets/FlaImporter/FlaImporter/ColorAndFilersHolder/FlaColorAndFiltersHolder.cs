using System;
using System.Collections.Generic;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder.ColorTransform;
using Assets.FlaImporter.FlaImporter.ColorAndFilersHolder.Enums;
using Assets.FlaImporter.FlaImporter.Renderer;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.FlaImporter.FlaImporter.ColorAndFilersHolder
{
    [ExecuteInEditMode]
    public class FlaColorAndFiltersHolder : MonoBehaviour
    {
        public static readonly Dictionary<FlaColorAndFiltersHolderPropertyTypeEnum,string> PropertyNames = new Dictionary<FlaColorAndFiltersHolderPropertyTypeEnum, string>
        {
            {FlaColorAndFiltersHolderPropertyTypeEnum.SelfColorTransform,"_selfColorTransform"}    
        };


        [SerializeField]
        private FlaColorAndFiltersHolder _parent;
        public FlaColorAndFiltersHolder Parent
        {
            get { return _parent; }
            private set
            {
                _parent = value;
                UpdateChilds();
            }
        }

        [SerializeField]
        private List<FlaColorAndFiltersHolder> _childs = new List<FlaColorAndFiltersHolder>();

        [SerializeField]
        private List<FlaShape> _shapes = new List<FlaShape>();

        [SerializeField]
        private FlaColorTransform _selfColorTransform = new FlaColorTransform(Vector4.zero, Color.white, new Color(0,0,0,0),0,0);
        public FlaColorTransform SelfColorTransform
        {
            get { return _selfColorTransform; }
            set
            {
                _selfColorTransform = value; 
                UpdateChilds();
            }
        }

        private FlaColorTransform _globalColorTransform = new FlaColorTransform(Vector4.zero, Color.white, new Color(0,0,0,0),0,0);
        public FlaColorTransform GlobalColorTransform
        {
            get
            {
                _globalColorTransform.CopyFrom(SelfColorTransform);
                if (_parent != null)
                {
                    _globalColorTransform.Concat(_parent.GlobalColorTransform); 
                }
                return _globalColorTransform;
            }
        }

        public FlaColorAndFiltersHolderMaskType MaskType = FlaColorAndFiltersHolderMaskType.Simple;
        public int MaskId = -1;

        public FlaColorAndFiltersHolderGuidType GuidType = FlaColorAndFiltersHolderGuidType.Simple;
        public int GuidId = -1;

        public void AddChild(FlaColorAndFiltersHolder child)
        {
            if (_childs == null)
            {
                _childs = new List<FlaColorAndFiltersHolder>();
            }
            if (!_childs.Contains(child))
            {
                _childs.Add(child);
                child.Parent = this;    
            }
        }

        public void RemoveChild(FlaColorAndFiltersHolder child)
        {
            _childs.Remove(child);
            child.Parent = null;
        }

        public void AddShape(FlaShape shape)
        {
            if (_shapes == null)
            {
                _shapes = new List<FlaShape>();
            }
            if (!_shapes.Contains(shape))
            {
                _shapes.Add(shape);
                shape.UpdateColorTranform(GlobalColorTransform);
            }
        }

        public void RemoveShape(FlaShape shape)
        {
            _shapes.Remove(shape);
        }

        private void LateUpdate()
        {
            if (_selfColorTransform.UpdateTint() || _selfColorTransform.UpdateBrightness() || _selfColorTransform.UpdateColorTransform())
            {
                UpdateChilds();
            }
        }

        private void OnEnable()
        {
            if (Parent == null)
            {
                UpdateChilds();
            }
        }

        private void UpdateChilds()
        {
            foreach (var filtersHolder in _childs)
            {
                filtersHolder.UpdateChilds();
            }

            if (_shapes != null && _shapes.Count > 0)
            {
                for (int i = 0; i < _shapes.Count; i++)
                {
                    _shapes[i].UpdateColorTranform(GlobalColorTransform);

                    for (int j = 0; j < _shapes[i].Edges.Count; j++)
                    {
                        var isMask = MaskType == FlaColorAndFiltersHolderMaskType.Mask;
                        _shapes[i].Edges[j].FillStyle.Material.SetInt("_MaskType",isMask ? 1:0);
                        _shapes[i].Edges[j].FillStyle.Material.renderQueue = isMask ? 1000 : 4000;
                        _shapes[i].Edges[j].FillStyle.Material.SetInt("_StencilId",MaskId);
                        _shapes[i].Edges[j].FillStyle.Material.SetInt("_StencilOp", (int)(isMask?StencilOp.Replace:StencilOp.Keep));
                        switch (MaskType)
                        {
                            default:
                            case FlaColorAndFiltersHolderMaskType.Simple:
                                _shapes[i].Edges[j].FillStyle.Material.SetInt("_StencilComp", (int) CompareFunction.Disabled);
                                _shapes[i].Edges[j].FillStyle.Material.SetInt("_ColorMask", (int)ColorWriteMask.All);
                                break;
                            case FlaColorAndFiltersHolderMaskType.Mask:
                                _shapes[i].Edges[j].FillStyle.Material.SetInt("_StencilComp", (int)CompareFunction.Always);
                                _shapes[i].Edges[j].FillStyle.Material.SetInt("_ColorMask", 0);
                                break;
                            case FlaColorAndFiltersHolderMaskType.Masked:
                                _shapes[i].Edges[j].FillStyle.Material.SetInt("_StencilComp", (int)CompareFunction.Equal);
                                _shapes[i].Edges[j].FillStyle.Material.SetInt("_ColorMask", (int)ColorWriteMask.All);
                                break;
                            
                                
                        }
                        
                        //StencilOp.
                        //CompareFunction.Always
                    }
                }
            }
        }
    }

    public enum FlaColorAndFiltersHolderGuidType
    {
        Simple,
        Guid,
        Guided
    }

    public enum FlaColorAndFiltersHolderMaskType
    {
        Simple,
        Mask,
        Masked,
    }
}
