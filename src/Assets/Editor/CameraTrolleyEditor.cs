using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraTrolley))]
public class CameraTrolleyEditor : Editor
{
  private GUIStyle _style = new GUIStyle();

  private CameraTrolley _target;

  private int _selectedHandleIndex = -1;

  void OnEnable()
  {
    _style.fontStyle = FontStyle.Bold;

    _style.normal.textColor = Color.white;

    _target = (CameraTrolley)target;
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
        _target.Nodes.Add(new Vector3(
          _target.Nodes[_target.Nodes.Count - 1].x + 10f,
          _target.Nodes[_target.Nodes.Count - 1].y,
          _target.Nodes[_target.Nodes.Count - 1].z));
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
      Undo.RecordObject(_target, "Adjust Linear Path");

      Handles.Label(_target.gameObject.transform.TransformPoint(_target.Nodes[0]), "'" + _target.name + "' Begin", _style);
      Handles.Label(_target.gameObject.transform.TransformPoint(_target.Nodes[_target.Nodes.Count - 1]), "'" + _target.name + "' End", _style);

      var hashCode = GetHashCode();

      //node handle display:
      for (var i = 0; i < _target.Nodes.Count; i++)
      {
        var controlIDBeforeHandle = GUIUtility.GetControlID(hashCode, FocusType.Passive);

        var isEventUsedBeforeHandle = (Event.current.type == EventType.used);

        _target.Nodes[i] = _target.gameObject.transform.InverseTransformPoint(
          Handles.PositionHandle(_target.gameObject.transform.TransformPoint(_target.Nodes[i]), Quaternion.identity));

        var controlIDAfterHandle = GUIUtility.GetControlID(hashCode, FocusType.Passive);
        var isEventUsedByHandle = !isEventUsedBeforeHandle && (Event.current.type == EventType.used);

        if ((controlIDBeforeHandle < GUIUtility.hotControl && GUIUtility.hotControl < controlIDAfterHandle) || isEventUsedByHandle)
        {
          _selectedHandleIndex = i;
        }
      }
    }

    var currentEvent = Event.current;

    switch (currentEvent.type)
    {
      case EventType.keyDown:

        if (Event.current.keyCode == (KeyCode.Delete))
        {
          Debug.Log(_selectedHandleIndex);

          if (_selectedHandleIndex >= 0)
          {
            currentEvent.Use();

            _target.Nodes.RemoveAt(_selectedHandleIndex);
            _target.NodeCount = _target.Nodes.Count;
          }
        }

        break;
    }
  }
}