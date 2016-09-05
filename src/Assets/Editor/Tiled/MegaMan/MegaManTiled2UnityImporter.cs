using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tiled2Unity;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  [CustomTiledImporter]
  class MegaManTiled2UnityImporter : ICustomTiledImporter
  {
    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props)
    {
    }

    public void CustomizePrefab(GameObject prefab)
    {
      CustomizeSafe(prefab);
    }

    private void Customize(GameObject prefab)
    {
      Destroy(
        prefab,
        "Rooms",
        "Enemies",
        "Checkpoints",
        "Camera Modifier");

      if (prefab != null)
      {
        AttachCustomObjects(prefab);
      }

      LinkCheckpointsToRooms(prefab);
    }

    private void CustomizeSafe(GameObject prefab)
    {
      try
      {
        Customize(prefab);
      }
      catch (Exception ex)
      {
        Debug.LogError(ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace ?? string.Empty);
      }
    }

    private void LinkCheckpointsToRooms(GameObject prefab)
    {
      var checkpoints = prefab.GetComponentsInChildren<Checkpoint>();
      var rooms = prefab.GetComponentsInChildren<FullScreenScroller>();
      var cameraModifiers = prefab.GetComponentsInChildren<CameraModifier>();

      foreach (var checkpoint in checkpoints)
      {
        var roomTransforms = rooms
          .Where(r => r.Contains(checkpoint.transform.position))
          .Select(r => r.transform);

        var cameraModifierTransforms = cameraModifiers
          .Where(c => c.Contains(checkpoint.transform.position))
          .Select(c => c.transform);

        var parentTransform = roomTransforms.Union(cameraModifierTransforms).FirstOrDefault();

        if (parentTransform == null)
        {
          throw new Exception("Checkpoint " + checkpoint.name + " must be within a room");
        }

        checkpoint.transform.parent = parentTransform;
      }
    }

    private void AttachCustomObjects(GameObject prefab)
    {
      var scene = EditorSceneManager.GetActiveScene();

      var directory = Path.GetDirectoryName(scene.path);

      var tmxFile = Path.Combine(
        directory,
        prefab.name + ".tmx");

      var objectTypesPath = "Assets/Tiled/objecttypes.xml";

      Debug.Log("Importing file '" + tmxFile + "'");

      var importer = TiledProjectImporter.CreateFromFile(
        tmxFile,
        objectTypesPath);

      importer.Import(prefab);
    }

    private void Destroy(GameObject prefab, params string[] names)
    {
      foreach (var name in names)
      {
        var childTransform = prefab.transform.FindChild(name);

        while (childTransform != null)
        {
          Debug.Log("Tile2Unity Import: Destroying game object " + name);

          UnityEngine.Object.DestroyImmediate(childTransform.gameObject);

          childTransform = prefab.transform.FindChild(name);
        }
      }
    }
  }
}
