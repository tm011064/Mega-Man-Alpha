using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MultiWayCameraModifier))]
[CanEditMultipleObjects]
public class MultiWayCameraModifierEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    var script = (MultiWayCameraModifier)target;

    if (GUILayout.Button("Build Object"))
    {
      script.BuildObject();
    }

    if (GUILayout.Button("Import Settings"))
    {
      script.ImportSettings();
    }
  }
}
