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
      try
      {
        Destroy(
          prefab,
          "Rooms",
          "Enemies",
          "Checkpoints");

        if (prefab != null)
        {
          AttachCustomObjects(prefab);
        }

        LinkCheckpointsToRooms(prefab);
      }
      catch (Exception ex)
      {
        Debug.LogError(ex.Message);
      }
    }

    private void LinkCheckpointsToRooms(GameObject prefab)
    {
      var checkpoints = prefab.GetComponentsInChildren<Checkpoint>();
      var rooms = prefab.GetComponentsInChildren<FullScreenScroller>();

      foreach (var checkpoint in checkpoints)
      {
        var room = rooms
          .Where(r => r.Contains(checkpoint.transform.position))
          .FirstOrDefault();

        if (room == null)
        {
          throw new Exception("Checkpoint " + checkpoint.name + " must be within a room");
        }

        checkpoint.transform.parent = room.transform;
      }
    }

    private void AttachCustomObjects(GameObject prefab)
    {
      var scene = EditorSceneManager.GetActiveScene();

      var tmxPath = Path.Combine(
        Path.GetDirectoryName(scene.path),
        Path.GetFileNameWithoutExtension(scene.path) + ".tmx");

      var objectTypesPath = "Assets/Tiled/objecttypes.xml";

      var importer = TiledProjectImporter.CreateFromFile(
        tmxPath,
        objectTypesPath);

      importer.Import(prefab);
    }

    private void Destroy(GameObject prefab, params string[] names)
    {
      foreach (var name in names)
      {
        var childTransform = prefab.transform.FindChild(name);

        if (childTransform != null)
        {
          Debug.Log("Tile2Unity Import: Destroying game object " + name);

          UnityEngine.Object.DestroyImmediate(childTransform.gameObject);
        }
      }
    }
  }
}
