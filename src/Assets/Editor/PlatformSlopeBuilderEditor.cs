using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlatformSlopeBuilderScript))]
public class PlatformSlopeBuilderEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    var script = (PlatformSlopeBuilderScript)target;

    if (GUILayout.Button("Build Object"))
    {
      script.BuildObject();
    }
  }
}
