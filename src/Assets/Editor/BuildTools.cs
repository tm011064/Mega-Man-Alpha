#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildTools
{
  public static void BuildWeb()
  {
    Build(@"AutoBuild\WebBuild.log", BuildTarget.WebPlayer, @"AutoBuild\Builds\Web\" + PlayerSettings.productName);
  }

  public static void BuildPC()
  {
    Build(@"AutoBuild\PCBuild.log", BuildTarget.StandaloneWindows, @"AutoBuild\Builds\Win32\" + PlayerSettings.productName + ".exe");
  }

  public static void BuildAndroid()
  {
    Build(@"AutoBuild\AndroidBuild.log", BuildTarget.Android, @"AutoBuild\Builds\Android\" + PlayerSettings.productName + ".apk");
  }

  public static void Build(string log_filename, BuildTarget target, string output)
  {
    using (var log = new LogFile(log_filename, false))
    {
      log.Message("Building Platform: " + target.ToString());
      log.Message("");

      string[] level_list = FindScenes();

      log.Message("Scenes to be processed: " + level_list.Length);

      foreach (string s in level_list)
      {
        var levelName = s.Remove(s.IndexOf(".unity"));
        log.Message("   " + levelName);
      }

      // Make sure the paths exist before building.
      try
      {
        var directoryName = output.Substring(0, output.LastIndexOf('\\'));

        Debug.Log("Checking whether directory " + directoryName + " exists...");

        var directoryInfo = new DirectoryInfo(directoryName);
        if (!directoryInfo.Exists)
        {
          Debug.Log("Create directory " + directoryName);

          directoryInfo.Create();
        }
      }
      catch
      {
        Debug.LogError("Failed to create directories: " + new DirectoryInfo(output).FullName);
      }

      var results = BuildPipeline.BuildPlayer(level_list, output, target, BuildOptions.None);
      log.Message("");

      if (results.Length == 0)
      {
        log.Message("No Build Errors");
      }
      else
      {
        log.Message("Build Error:" + results);
      }
    }
  }

  private static int CountScenes()
  {
    int totalScenes = 0;

    foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
    {
      if (scene.enabled)
      {
        totalScenes++;
      }
    }

    return totalScenes;
  }

  public static string[] FindScenes()
  {
    int totalScenes = CountScenes();

    var scenes = new string[totalScenes];

    int index = 0;

    foreach (var scene in EditorBuildSettings.scenes)
    {
      if (scene.enabled)
      {
        scenes[index++] = scene.path;
      }
    }

    return (scenes);
  }
}

#endif
