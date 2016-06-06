using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArcEdgeColliderBuildScript))]
public class ArcEdgeColliderBuilderEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    var script = (ArcEdgeColliderBuildScript)target;

    if (GUILayout.Button("Build Object"))
    {
      script.BuildObject();
    }
  }
}
