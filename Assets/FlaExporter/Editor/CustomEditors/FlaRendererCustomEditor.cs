using Assets.FlaExporter.FlaExporter.Renderer;
using UnityEditor;

namespace Assets.FlaExporter.Editor.CustomEditors
{
    [CustomEditor(typeof(FlaEdge))]
    public class FlaRendererCustomEditor : UnityEditor.Editor
    {
        public FlaEdge TargetRenderer
        {
            get { return (FlaEdge) target; }
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
