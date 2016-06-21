using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneSwitchWindow : EditorWindow
{
  private Vector2 _scrollPos;

  [MenuItem("Tools/Switch Scene")]
  internal static void Init()
  {
    // EditorWindow.GetWindow() will return the open instance of the specified window or create a new
    // instance if it can't find one. The second parameter is a flag for creating the window as a
    // Utility window; Utility windows cannot be docked like the Scene and Game view windows.
    var window = (SceneSwitchWindow)GetWindow(typeof(SceneSwitchWindow), false, "Scene Switch");

    window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 768f, 400f);
  }

  internal void OnGUI()
  {
    EditorGUILayout.BeginVertical();

    this._scrollPos = EditorGUILayout.BeginScrollView(this._scrollPos, false, false);

    GUILayout.Label("Scenes In Build", EditorStyles.boldLabel);

    Debug.Log(Application.dataPath);

    var fileInfos = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories)
      .Select(f => new FileInfo(f))
      .OrderByDescending(f => f.LastWriteTimeUtc);

    foreach (FileInfo fileInfo in fileInfos)
    {
      var name = fileInfo
        .FullName
        .Replace(Application.dataPath, string.Empty)
        .TrimStart('\\')
        .TrimStart('/');

      name = name.Substring(0, name.LastIndexOf('.'));

      var pressed = GUILayout.Button(
        name,
        new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft });

      if (pressed)
      {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
          EditorSceneManager.OpenScene(fileInfo.FullName);

          EditorWindow.GetWindow(typeof(SceneView)).Show();
        }
      }
    }

    EditorGUILayout.EndScrollView();

    EditorGUILayout.EndVertical();
  }
}