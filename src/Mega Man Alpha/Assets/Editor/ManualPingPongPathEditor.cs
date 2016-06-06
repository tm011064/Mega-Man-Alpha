using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ManualPingPongPath))]
public class ManualPingPongPathEditor : Editor
{
  private GUIStyle _style = new GUIStyle();

  private ManualPingPongPath _target;

  void OnEnable()
  {
    _style.fontStyle = FontStyle.Bold;

    _style.normal.textColor = Color.white;

    _target = (ManualPingPongPath)target;
  }

  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    if (_target.NodeCount < 2)
    {
      _target.NodeCount = 2;
    }

    if (_target.NodeCount > _target.Nodes.Count)
    {
      for (var i = 0; i < _target.NodeCount - _target.Nodes.Count; i++)
      {
        _target.Nodes.Add(Vector3.zero);
      }
    }

    if (_target.NodeCount < _target.Nodes.Count)
    {
      if (EditorUtility.DisplayDialog("Remove path node?", "Shortening the node list will permantently destory parts of your path. This operation cannot be undone.", "OK", "Cancel"))
      {
        var removeCount = _target.Nodes.Count - _target.NodeCount;
        _target.Nodes.RemoveRange(_target.Nodes.Count - removeCount, removeCount);
      }
      else
      {
        _target.NodeCount = _target.Nodes.Count;
      }
    }

    EditorGUI.indentLevel = 4;
    for (var i = 0; i < _target.Nodes.Count; i++)
    {
      _target.Nodes[i] = EditorGUILayout.Vector3Field("Node " + (i + 1), _target.Nodes[i]);
    }

    if (GUI.changed)
    {
      EditorUtility.SetDirty(_target);
    }
  }

  void OnSceneGUI()
  {
    if (_target.Nodes.Count > 0)
    {
      //allow path adjustment undo:
      Undo.RecordObject(_target, "Adjust DynamicPingPong Path");

      Handles.Label(_target.gameObject.transform.TransformPoint(_target.Nodes[0]), "'" + _target.name + "' Begin", _style);
      Handles.Label(_target.gameObject.transform.TransformPoint(_target.Nodes[_target.Nodes.Count - 1]), "'" + _target.name + "' End", _style);

      //node handle display:
      for (var i = 0; i < _target.Nodes.Count; i++)
      {
        _target.Nodes[i] = _target.gameObject.transform.InverseTransformPoint(
          Handles.PositionHandle(_target.gameObject.transform.TransformPoint(_target.Nodes[i]), Quaternion.identity));
      }
    }
  }
}