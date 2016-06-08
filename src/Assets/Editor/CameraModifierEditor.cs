using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraModifier))]
public class CameraModifierEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    var script = (CameraModifier)target;

    if (GUILayout.Button("Import Settings"))
    {
      script.ImportSettings();
    }
  }
}