using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class OneWayPlatformColliderFactory : AbstractGameObjectFactory
  {
    public OneWayPlatformColliderFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      return Map
        .ForEachLayerWithProperty("Collider", "onewayplatform")
        .Get<GameObject>(CreateColliders);
    }

    private GameObject CreateColliders(Layer layer)
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
  }
}
