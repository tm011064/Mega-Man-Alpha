using UnityEditor;
using UnityEngine;

public class AutoSnap : EditorWindow
{
  private Vector3 _prevPosition;

  private bool _doSnap = true;

  private float _snapValue = 1;

  [MenuItem("Tools/Auto Snap %_l")]
  static void Init()
  {
    var window = (AutoSnap)EditorWindow.GetWindow(typeof(AutoSnap));

    window.maxSize = new Vector2(200, 100);
  }

  void Awake()
  {
    SceneView.onSceneGUIDelegate -= OnSceneGUI;

    SceneView.onSceneGUIDelegate += OnSceneGUI;
  }

  public void OnGUI()
  {
    _doSnap = EditorGUILayout.Toggle("Auto Snap", _doSnap);

    _snapValue = EditorGUILayout.FloatField("Snap Value", _snapValue);
  }

  void OnFocus()
  {
    SceneView.onSceneGUIDelegate -= OnSceneGUI;

    SceneView.onSceneGUIDelegate += OnSceneGUI;
  }

  void OnDestroy()
  {
    SceneView.onSceneGUIDelegate -= OnSceneGUI;
  }

  void OnSceneGUI(SceneView sceneView)
  {
    if (_doSnap
      && !EditorApplication.isPlaying
      && Selection.transforms.Length > 0
      && Selection.transforms[0].position != _prevPosition)
    {
      Snap();

      _prevPosition = Selection.transforms[0].position;
    }
  }

  private void Snap()
  {
    foreach (var transform in Selection.transforms)
    {
      var t = transform.transform.position;

      t.x = Round(t.x);
      t.y = Round(t.y);
      t.z = Round(t.z);

      transform.transform.position = t;
    }
  }

  private float Round(float input)
  {
    return _snapValue * Mathf.Round((input / _snapValue));
  }
}