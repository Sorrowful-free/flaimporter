﻿using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform;
using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.Enums;
using Assets.FlaExporter.FlaExporter.Renderer;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.ColorAndFilersHolder
{
    [ExecuteInEditMode]
    public class FlaColorAndFiltersHolder : MonoBehaviour
    {
        public static readonly Dictionary<FlaColorAndFiltersHolderPropertyTypeEnum,string> PropertyNames = new Dictionary<FlaColorAndFiltersHolderPropertyTypeEnum, string>
        {
            {FlaColorAndFiltersHolderPropertyTypeEnum.SelfColorTransform,"_selfColorTransform"}    
        };

        [SerializeField] 
        public FlaShape FlaShape;
      

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

        public void AddChild(FlaColorAndFiltersHolder child)
        {
            if (_childs == null)
            {
                _childs = new List<FlaColorAndFiltersHolder>();
            }
            _childs.Add(child);
            child.Parent = this;
        }

        public void RemoveChild(FlaColorAndFiltersHolder child)
        {
            _childs.Remove(child);
            child.Parent = null;
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

            if (FlaShape != null)
            {
                FlaShape.UpdateColorTranform(GlobalColorTransform);
            }
        }

    }
}
