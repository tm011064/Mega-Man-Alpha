using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RectangleMeshBuildScript))]
public class RectangleMeshBuilderEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    var script = (RectangleMeshBuildScript)target;

    script.SafeDeleteColliders();

    if (GUILayout.Button("Build Object"))
    {
      script.BuildObject();
    }

    if (GUILayout.Button("Create Prefab"))
    {
      script.CreatePrefab();
    }
  }
}