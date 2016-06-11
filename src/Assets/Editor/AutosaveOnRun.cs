using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class AutosaveOnRun
{
  static AutosaveOnRun()
  {
    EditorApplication.playmodeStateChanged = () =>
    {
      if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
      {
        Debug.Log("Auto-Saving scene before entering Play mode: " + EditorSceneManager.GetActiveScene().name);

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        
        EditorApplication.SaveAssets();
      }
    };
  }
}