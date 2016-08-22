﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public class TiledProjectImporter
  {
    private readonly Map _map;

    private readonly Dictionary<string, Objecttype> _objecttypesByName;

    private readonly List<Objecttype> _objecttypesWithPrefabProperty;

    private Dictionary<string, string> _prefabLookup = new Dictionary<string, string>();

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
      _map = map;

      _objecttypesByName = objecttypes
        .Objecttype
        .ToDictionary(ot => ot.Name, ot => ot, StringComparer.InvariantCultureIgnoreCase);
    }

    public void Import(GameObject parent = null)
    {
      _prefabLookup = AssetDatabase
        .GetAllAssetPaths()
        .Where(path => path.EndsWith(".prefab"))
        .ToDictionary(p => GetPrefabName(p), p => p, StringComparer.InvariantCultureIgnoreCase);

      var tiledObjectsGameObject = new GameObject("Tiled Objects");
      tiledObjectsGameObject.transform.position = Vector3.zero;

      tiledObjectsGameObject.AttachChildren(
        _map.ForEachLayerWithProperty("Collider", "platform").Get<GameObject>(CreatePlatformColliders));

      tiledObjectsGameObject.AttachChildren(
        _map.ForEachLayerWithProperty("Collider", "onewayplatform").Get<GameObject>(CreateOneWayPlatformColliders));

      tiledObjectsGameObject.AttachChildren(
        _map.ForEachLayerWithProperty("Collider", "deathhazard").Get<GameObject>(CreateDeathHazardColliders));

      tiledObjectsGameObject.AttachChild(
        CreateLayerPrefabGameObjects());

      tiledObjectsGameObject.AttachChild(
        CreateTiledObjectPrefabs());

      tiledObjectsGameObject.AttachChildren(
        _map
          .ForEachObjectGroupWithProperty("Collider", "cameramodifier")
          .Get<GameObject>(CreateCameraModifers));

      if (parent != null)
      {
        tiledObjectsGameObject.transform.parent = parent.transform;
      }
    }

    private GameObject CreateCameraModifers(Objectgroup objectgroup)
    {
      var cameraBoundsObject = objectgroup.GetTypeOrThrow("Camera Bounds");
      var cameraModiferObjects = objectgroup.GetTypesOrThrow("Camera Trigger");

      var isFullScreenScroller = cameraModiferObjects.All(o => o.HasProperty(
        "Triggers Room Transition",
        "true",
        _objecttypesByName));

      return isFullScreenScroller
        ? CreateCameraModifier(
            cameraModiferObjects,
            cameraBoundsObject,
            "Full Screen Scroller With Edge Trigger",
            (cameraModifierBounds, cameraBounds, vector) => new Vector2(
                  cameraModifierBounds.center.x - cameraBounds.center.x + vector.x,
                  cameraModifierBounds.center.y - cameraBounds.center.y + vector.y))
        : CreateCameraModifier(
            cameraModiferObjects,
            cameraBoundsObject,
            "Camera Modifier",
            (cameraModifierBounds, cameraBounds, vector) => new Vector2(
                  cameraModifierBounds.center.x + vector.x,
                  cameraModifierBounds.center.y - vector.y));
    }

    private GameObject CreateCameraModifier(
      Object[] cameraModiferObjects,
      Object cameraBoundsObject,
      string prefabName,
      Func<Bounds, Bounds, Vector2, Vector2> vectorFactory)
    {
      if (cameraModiferObjects.Any(o => o.PolyLine == null))
      {
        throw new Exception("Camera Modifier objects must have a poly line tag");
      }

      var cameraBounds = cameraBoundsObject.GetBounds();

      var lines = cameraModiferObjects
        .Select(o => new { Bounds = o.GetBounds(), Object = o })
        .Select(r => r.Object
          .PolyLine
          .ToVectors()
          .Select(v => vectorFactory(r.Bounds, cameraBounds, v))
          .ToArray())
        .Select(v => new Line2(v))
        .ToArray();

      var asset = LoadPrefabAsset(prefabName);

      return CreateInstantiableObject(
       asset,
       prefabName,
       new InstantiationArguments
       {
         Bounds = cameraBounds,
         Lines = lines,
         Arguments = cameraModiferObjects.First().GetProperties(_objecttypesByName) // TODO (Roman): this is not nice
       });
    }

    private string GetPrefabName(string assetPath)
    {
      var fileInfo = new FileInfo(assetPath);

      return fileInfo.Name.Remove(fileInfo.Name.Length - (".prefab".Length));
    }

    private GameObject CreateTiledObjectPrefabs()
    {
      var prefabsParent = new GameObject("Auto created Tiled prefabs");
      prefabsParent.transform.position = Vector3.zero;

      Debug.Log(_objecttypesByName);

      var prefabGameObjects = _map
        .ForEachObjectWithProperty("Prefab", _objecttypesByName)
        .Get<GameObject>(CreatePrefabFromGameObject);

      foreach (var gameObject in prefabGameObjects)
      {
        gameObject.transform.parent = prefabsParent.transform;
      }

      return prefabsParent;
    }

    private GameObject CreateLayerPrefabGameObjects()
    {
      var parentObject = new GameObject("Auto created Tiled layer objects");

      parentObject.transform.position = Vector3.zero;

      var createdGameObjects = _map
        .ForEachLayerWithPropertyName("Prefab")
        .Get<IEnumerable<GameObject>>(CreatePrefabsFromLayer)
        .SelectMany(l => l);

      foreach (var gameObject in createdGameObjects)
      {
        gameObject.transform.parent = parentObject.transform;
      }

      return parentObject;
    }

    private GameObject CreatePrefabFromGameObject(Object obj)
    {
      var properties = obj.GetProperties(_objecttypesByName);

      var prefabName = properties["prefab"];

      var asset = LoadPrefabAsset(prefabName);

      return CreateInstantiableObject(
       asset,
       prefabName,
       new InstantiationArguments
       {
         Bounds = obj.GetBounds(),
         Arguments = properties,
         IsFlippedHorizontally = obj.Gid >= 2000000000,
         IsFlippedVertically = (obj.Gid >= 1000000000 && obj.Gid < 2000000000) || obj.Gid >= 3000000000
       });
    }

    private IEnumerable<GameObject> CreatePrefabsFromLayer(Layer layer)
    {
      var prefabName = layer.Properties
        .Property
        .First(p => string.Compare(p.Name.Trim(), "Prefab", true) == 0)
        .Value
        .Trim()
        .ToLower();

      var asset = LoadPrefabAsset(prefabName);

      var matrixVertices = CreateMatrixVertices(layer);

      foreach (var bounds in matrixVertices.GetRectangleBounds())
      {
        yield return CreateInstantiableObject(
          asset,
          prefabName,
          new InstantiationArguments
          {
            Bounds = bounds,
            Arguments = layer
              .Properties
              .Property
              .ToDictionary(p => p.Name, p => p.Value, StringComparer.InvariantCultureIgnoreCase)
          });
      }
    }

    private GameObject LoadPrefabAsset(string prefabName)
    {
      string assetPath;

      if (!_prefabLookup.TryGetValue(prefabName, out assetPath))
      {
        throw new MissingReferenceException("No prefab with name '" + prefabName + "' exists at this project");
      }

      return AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
    }

    private GameObject CreateInstantiableObject(GameObject asset, string prefabName, InstantiationArguments arguments)
    {
      var gameObject = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity) as GameObject;

      gameObject.name = prefabName;

      var instantiable = gameObject.GetComponentOrThrow<IInstantiable>();

      instantiable.Instantiate(arguments);

      return gameObject;
    }

    private MatrixVertices CreateMatrixVertices(Layer layer)
    {
      var tileNumbers = new long[_map.Height * _map.Width];

      var tileNumbersFlipped = layer
        .Data
        .Text
        .Split(',')
        .Select(text => long.Parse(text));

      var rowIndex = _map.Height - 1;
      var columnIndex = 0;

      foreach (var value in tileNumbersFlipped)
      {
        var location = rowIndex * _map.Width + columnIndex;

        tileNumbers[location] = value;

        columnIndex++;

        if (columnIndex == _map.Width)
        {
          columnIndex = 0;
          rowIndex--;
        }
      }

      var matrix = new Matrix<long>(tileNumbers, _map.Height, _map.Width);

      return new MatrixVertices(matrix, _map.Tilewidth, _map.Tileheight);
    }

    private GameObject CreateDeathHazardColliders(Layer layer)
    {
      var vertices = CreateMatrixVertices(layer);

      var collidersGameObject = new GameObject("Death Hazard Colliders");
      collidersGameObject.transform.position = Vector3.zero;

      int padding;
      layer.TryGetProperty("Collider Padding", out padding);

      var colliders = vertices.GetColliderEdges(padding);

      foreach (var points in colliders)
      {
        var obj = new GameObject("Death Hazard Trigger Collider");

        obj.transform.position = Vector3.zero;
        obj.layer = LayerMask.NameToLayer("PlayerTriggerMask");

        AddEdgeColliders(obj, points, padding, true);

        obj.AddComponent<InstantDeathHazardTrigger>();

        obj.transform.parent = collidersGameObject.transform;
      }

      return collidersGameObject;
    }

    private GameObject CreatePlatformColliders(Layer layer)
    {
      var vertices = CreateMatrixVertices(layer);

      var collidersGameObject = new GameObject("Platform Colliders");
      collidersGameObject.transform.position = Vector3.zero;

      var colliders = vertices.GetColliderEdges();

      foreach (var points in colliders)
      {
        var obj = new GameObject("Collider");

        obj.transform.position = Vector3.zero;
        obj.layer = LayerMask.NameToLayer("Platforms");

        AddEdgeColliders(obj, points);

        obj.transform.parent = collidersGameObject.transform;
      }

      return collidersGameObject;
    }

    private GameObject CreateOneWayPlatformColliders(Layer layer)
    {
      var vertices = CreateMatrixVertices(layer);

      var collidersGameObject = new GameObject("One Way Platform Colliders");
      collidersGameObject.transform.position = Vector3.zero;

      var edgePoints = vertices.GetTopColliderEdges();

      foreach (var edge in edgePoints)
      {
        var edgeColliderObject = new GameObject("Edge Collider");

        var extents = new Vector2(
          edge.To.x - edge.From.x,
          edge.To.y - edge.From.y) * .5f;

        edgeColliderObject.transform.position = new Vector3(
          edge.To.x - extents.x,
          edge.To.y - extents.y);

        edgeColliderObject.layer = LayerMask.NameToLayer("OneWayPlatform");

        var edgeCollider = edgeColliderObject.AddComponent<EdgeCollider2D>();

        edgeCollider.points = new Vector2[] 
        { 
          new Vector2(-extents.x, extents.y),
          new Vector2(extents.x, extents.y)
        };

        edgeColliderObject.transform.parent = collidersGameObject.transform;
      }

      return collidersGameObject;
    }

    private void AddEdgeColliders(GameObject parent, Vector2[] points, int padding = 0, bool isTrigger = false)
    {
      for (var i = 0; i < points.Length - 1; i++)
      {
        AddEdgeCollider(parent, points[i], points[i + 1], isTrigger);
      }

      AddEdgeCollider(parent, points.Last(), points.First(), isTrigger);
    }

    private void AddEdgeCollider(GameObject parent, Vector2 from, Vector2 to, bool isTrigger)
    {
      var edgeCollider = parent.AddComponent<EdgeCollider2D>();

      edgeCollider.isTrigger = isTrigger;
      edgeCollider.points = new Vector2[] { from, to };
    }
  }
}
