using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class DeathHazardFactory : AbstractGameObjectFactory
  {
    public DeathHazardFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      return Map
        .ForEachLayerWithProperty("Collider", "deathhazard")
        .Get<GameObject>(CreateColliders);
    }

    private GameObject CreateColliders(Layer layer)
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
  }
}
