using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class LayerPrefabFactory : AbstractGameObjectFactory
  {
    public LayerPrefabFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      var parentObject = new GameObject("Auto created Tiled layer objects");

      parentObject.transform.position = Vector3.zero;

      var createdGameObjects = Map
        .ForEachLayerWithPropertyName("Prefab")
        .Get<IEnumerable<GameObject>>(CreatePrefabsFromLayer)
        .SelectMany(l => l);

      foreach (var gameObject in createdGameObjects)
      {
        gameObject.transform.parent = parentObject.transform;
      }

      yield return parentObject;
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
            Properties = layer
              .Properties
              .Property
              .ToDictionary(p => p.Name, p => p.Value, StringComparer.InvariantCultureIgnoreCase)
          });
      }
    }
  }
}
