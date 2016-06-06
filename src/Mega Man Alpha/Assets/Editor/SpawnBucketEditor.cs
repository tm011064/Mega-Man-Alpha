using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SpawnBucket))]
public class SpawnBucketEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    var script = (SpawnBucket)target;
    
    if (GUILayout.Button("Register Child Objects"))
    {
      script.RegisterChildObjects();
    }
  }
}