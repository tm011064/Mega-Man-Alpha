using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Assets.Editor.Tiled.GameObjectFactories;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public class TiledProjectImporter
  {
    private AbstractGameObjectFactory[] _gameObjectFactories;

    public static TiledProjectImporter CreateFromFile(string mapFilePath, string objectTypesFilePath = null)
    {
      var mapSerializer = new XmlSerializer(typeof(Map));

      Map map;

      Objecttypes objecttypes = new Objecttypes { Objecttype = new List<Objecttype>() };

      using (var reader = new StreamReader(mapFilePath))
      {
        map = (Map)mapSerializer.Deserialize(reader);
      }

      if (!string.IsNullOrEmpty(objectTypesFilePath))
      {
        var objecttypesSerializer = new XmlSerializer(typeof(Objecttypes));

        using (var reader = new StreamReader(objectTypesFilePath))
        {
          objecttypes = (Objecttypes)objecttypesSerializer.Deserialize(reader);
        }
      }

      return new TiledProjectImporter(map, objecttypes);
    }

    public TiledProjectImporter(Map map, Objecttypes objecttypes)
    {
      var objecttypesByName = objecttypes
        .Objecttype
        .ToDictionary(ot => ot.Name, ot => ot, StringComparer.InvariantCultureIgnoreCase);

      var prefabLookup = AssetDatabase
        .GetAllAssetPaths()
        .Where(path => path.EndsWith(".prefab"))
        .ToDictionary(p => GetPrefabName(p), p => p, StringComparer.InvariantCultureIgnoreCase);

      _gameObjectFactories = new AbstractGameObjectFactory[]
      {
        new PlatformColliderFactory(map, prefabLookup, objecttypesByName),
        new OneWayPlatformColliderFactory(map, prefabLookup, objecttypesByName),
        new DeathHazardFactory(map, prefabLookup, objecttypesByName),
        new LayerPrefabFactory(map, prefabLookup, objecttypesByName),
        new TiledObjectPrefabFactory(map, prefabLookup, objecttypesByName),
        new CameraModifierFactory(map, prefabLookup, objecttypesByName)
      };
    }

    public void Import(GameObject parent = null)
    {
      var tiledObjectsGameObject = new GameObject("Tiled Objects");
      tiledObjectsGameObject.transform.position = Vector3.zero;

      tiledObjectsGameObject.AttachChildren(
        _gameObjectFactories.SelectMany(f => f.Create()));

      if (parent != null)
      {
        tiledObjectsGameObject.transform.parent = parent.transform;
      }
    }

    private string GetPrefabName(string assetPath)
    {
      var fileInfo = new FileInfo(assetPath);

      return fileInfo.Name.Remove(fileInfo.Name.Length - (".prefab".Length));
    }
  }
}
