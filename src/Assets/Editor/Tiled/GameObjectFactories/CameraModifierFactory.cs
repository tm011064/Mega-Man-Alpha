using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class CameraModifierFactory : AbstractGameObjectFactory
  {
    public CameraModifierFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      return Map
        .ForEachObjectGroupWithProperty("Collider", "cameramodifier")
        .Get<GameObject>(CreateCameraModifers);
    }

    private GameObject CreateCameraModifers(Objectgroup objectgroup)
    {
      var cameraBoundsObject = objectgroup.GetTypeOrThrow("Camera Bounds");
      var cameraModiferObjects = objectgroup.GetTypesOrThrow("Camera Trigger");

      var isFullScreenScroller = cameraModiferObjects.All(o => o.HasProperty(
        "Triggers Room Transition",
        "true",
        ObjecttypesByName));

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
         Arguments = cameraModiferObjects.First().GetProperties(ObjecttypesByName) // TODO (Roman): this is not nice
       });
    }
  }
}
