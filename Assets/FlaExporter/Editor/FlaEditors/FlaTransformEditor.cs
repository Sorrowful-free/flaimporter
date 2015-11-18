using Assets.FlaExporter.FlaExporter;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.Editor.FlaEditors
{
    [CustomEditor(typeof(FlaTransform))]
    public class FlaTransformEditor : UnityEditor.Editor
    {
        private FlaTransform TargetTransform{get { return (FlaTransform) target; }}
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Undo.RecordObject(this, "Record FlaTransform Change");
            TargetTransform.Rotation = EditorGUILayout.FloatField("Rotation", TargetTransform.Rotation);
            Undo.RecordObject(this, "Record FlaTransform Change");
            TargetTransform.TransformPoint = EditorGUILayout.Vector2Field("TransformPoint", TargetTransform.TransformPoint);
            Undo.RecordObject(this, "Record FlaTransform Change");
            TargetTransform.Position = EditorGUILayout.Vector2Field("Position", TargetTransform.Position);
            Undo.RecordObject(this, "Record FlaTransform Change");
            TargetTransform.Scale = EditorGUILayout.Vector2Field("Scale", TargetTransform.Scale);
            Undo.RecordObject(this, "Record FlaTransform Change");
            TargetTransform.Skew = EditorGUILayout.Vector2Field("Skew", TargetTransform.Skew);
            Undo.RecordObject(this, "Record FlaTransform Change");
        }

        private void OnSceneGUI()
        {
            var localToGlobal = TargetTransform.transform.TransformPoint(TargetTransform.TransformPoint);

            localToGlobal = Handles.FreeMoveHandle(localToGlobal, Quaternion.identity,
                HandleUtility.GetHandleSize(localToGlobal)*0.05f, Vector3.one, Handles.CircleCap);
            TargetTransform.TransformPoint = TargetTransform.transform.InverseTransformPoint(localToGlobal);
        }


    }
}
