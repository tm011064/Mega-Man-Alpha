using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor {

	private void OnSceneGUI () {
		Line line = target as Line;
		Transform handleTransform = line.transform;
		Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
		Vector3 p0 = handleTransform.TransformPoint(line.From);
		Vector3 p1 = handleTransform.TransformPoint(line.To);

		Handles.color = Color.white;
		Handles.DrawLine(p0, p1);
		EditorGUI.BeginChangeCheck();
		p0 = Handles.DoPositionHandle(p0, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(line, "Move Point");
			EditorUtility.SetDirty(line);
			line.From = handleTransform.InverseTransformPoint(p0);
		}
		EditorGUI.BeginChangeCheck();
		p1 = Handles.DoPositionHandle(p1, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(line, "Move Point");
			EditorUtility.SetDirty(line);
			line.To = handleTransform.InverseTransformPoint(p1);
		}
	}
}