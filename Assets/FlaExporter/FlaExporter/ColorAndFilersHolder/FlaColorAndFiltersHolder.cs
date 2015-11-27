using System.Collections.Generic;
using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.ColorTransform;
using Assets.FlaExporter.FlaExporter.ColorAndFilersHolder.Enums;
using Assets.FlaExporter.FlaExporter.Renderer;
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
        private FlaRenderer _flaRenderer;
        public FlaRenderer FlaRenderer
        {
            get
            {
                if (_flaRenderer == null)
                {
                    _flaRenderer = GetComponent<FlaRenderer>();
                }
                return _flaRenderer;
            }
        }

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
        private FlaColorTransform _selfColorTransform = new FlaColorTransform();
        public FlaColorTransform SelfColorTransform
        {
            get { return _selfColorTransform; }
            set
            {
                _selfColorTransform = value; 
                UpdateChilds();
            }
        }
        
        public FlaColorTransform GlobalColorTransform
        {
            get { return Parent == null ? SelfColorTransform : SelfColorTransform * Parent.GlobalColorTransform; }
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
        
#if UNITY_EDITOR
         private void Update()
#elif
         private void LateUpdate()
#endif
        {
            if (_selfColorTransform.IsChange)
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

            if (FlaRenderer != null)
            {
                FlaRenderer.UpdateColorTranform(GlobalColorTransform);
            }
        }

    }
}
