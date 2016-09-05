using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class TiledObjectPrefabFactory : AbstractGameObjectFactory
  {
    public TiledObjectPrefabFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      var prefabsParent = new GameObject("Auto created Tiled prefabs");
      prefabsParent.transform.position = Vector3.zero;

      var prefabGameObjects = Map
        .ForEachObjectWithProperty("Prefab", ObjecttypesByName)
        .Get<GameObject>(CreatePrefabFromGameObject);

      foreach (var gameObject in prefabGameObjects)
      {
        gameObject.transform.parent = prefabsParent.transform;
      }

      yield return prefabsParent;
    }

    private GameObject CreatePrefabFromGameObject(Object obj)
    {
      var properties = obj.GetProperties(ObjecttypesByName);

      var prefabName = properties["prefab"];

      var asset = LoadPrefabAsset(prefabName);

      return CreateInstantiableObject(
       asset,
       prefabName,
       new InstantiationArguments
       {
         Bounds = obj.GetBounds(),
         Properties = properties,
         IsFlippedHorizontally = obj.Gid >= 2000000000,
         IsFlippedVertically = (obj.Gid >= 1000000000 && obj.Gid < 2000000000) || obj.Gid >= 3000000000
       });
    }
  }
}
