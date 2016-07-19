using UnityEditor;
using UnityEngine;

public class EdgeCollider2DEditor : EditorWindow
{
  [MenuItem("Tools/EdgeCollider2D Snap")]
  public static void ShowWindow()
  {
    EditorWindow.GetWindow(typeof(EdgeCollider2DEditor));
  }

  private EdgeCollider2D _edgeCollider;

  private Vector2[] _vertices = new Vector2[0];

  void OnGUI()
  {
    GUILayout.Label("EdgeCollider2D point editor", EditorStyles.boldLabel);

    _edgeCollider = (EdgeCollider2D)EditorGUILayout.ObjectField(
      "EdgeCollider2D to edit",
      _edgeCollider,
      typeof(EdgeCollider2D),
      true);

    if (_vertices.Length != 0)
    {
      for (int i = 0; i < _vertices.Length; ++i)
      {
        _vertices[i] = (Vector2)EditorGUILayout.Vector2Field("Element " + i, _vertices[i]);
      }
    }

    if (GUILayout.Button("Retrieve"))
    {
      _vertices = _edgeCollider.points;
    }

    if (GUILayout.Button("Set"))
    {
      _edgeCollider.points = _vertices;
    }
  }
}