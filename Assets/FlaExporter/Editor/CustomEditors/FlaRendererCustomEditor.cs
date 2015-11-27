using Assets.FlaExporter.FlaExporter.Renderer;
using UnityEditor;

namespace Assets.FlaExporter.Editor.CustomEditors
{
    [CustomEditor(typeof(FlaRenderer))]
    public class FlaRendererCustomEditor : UnityEditor.Editor
    {
        public FlaRenderer TargetRenderer
        {
            get { return (FlaRenderer) target; }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //foreach (var fillStyle in TargetRenderer.FillStyles)
          //  {
          //      fillStyle.DrawGUI();
          //  }
        }
    }
}
