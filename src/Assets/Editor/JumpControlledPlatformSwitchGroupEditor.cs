using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JumpControlledPlatformSwitchGroup))]
public class JumpControlledPlatformSwitchGroupEditor : Editor
{
  private GUIStyle _style = new GUIStyle();

  private JumpControlledPlatformSwitchGroup _target;

  void OnEnable()
  {
    _style.fontStyle = FontStyle.Bold;

    _style.normal.textColor = Color.white;

    _target = (JumpControlledPlatformSwitchGroup)target;
  }

  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    foreach (var item in _target.PlatformGroupPositions)
    {
      if (item.Positions.Count < 1)
      {
        item.Positions.Add(Vector3.zero);
      }
    }

        if (GUI.changed)
    {
      EditorUtility.SetDirty(_target);
    }
  }

  void OnSceneGUI()
  {
    foreach (var item in _target.PlatformGroupPositions)
    {
      //allow path adjustment undo:
      Undo.RecordObject(_target, "Adjust JumpControlledPlatformSwitchGroup");

      //node handle display:
      for (var i = 0; i < item.Positions.Count; i++)
      {
        item.Positions[i] = _target.gameObject.transform.InverseTransformPoint(
          Handles.PositionHandle(_target.gameObject.transform.TransformPoint(item.Positions[i]), Quaternion.identity));
      }
    }
  }
}