using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EasedSlopeBuildScript))]
public class EasedSlopeBuildScripterEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    var script = (EasedSlopeBuildScript)target;

    if (GUILayout.Button("Build Object"))
    {
      script.BuildObject();
    }
  }
}