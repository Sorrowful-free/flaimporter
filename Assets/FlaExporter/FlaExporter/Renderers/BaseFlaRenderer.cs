using UnityEngine;

namespace Assets.FlaExporter.FlaExporter.Renderers
{
    
    public class BaseFlaRenderer : MonoBehaviour
    {
        private Renderer _currentRenderer;

        private Renderer CurrentRenderer
        {
            get
            {
                if (_currentRenderer == null)
                {
                    _currentRenderer = GetComponent<Renderer>();
                }
                return _currentRenderer;
            }
        }

        public bool Visible
        {
            get { return CurrentRenderer.enabled; }
            set { CurrentRenderer.enabled = value; }
        }
    }
}
