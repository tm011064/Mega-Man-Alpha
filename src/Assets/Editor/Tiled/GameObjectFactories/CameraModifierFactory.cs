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
        .Get<IEnumerable<GameObject>>(CreateCameraModifers)
        .SelectMany(l => l);
    }

    private IEnumerable<GameObject> CreateCameraModifers(Objectgroup objectgroup)
    {
      var bounds = objectgroup.GetTypeOrThrow("Camera Bounds");
      var triggers = objectgroup.GetTypesOrThrow("Camera Trigger");

      var fullScreenScrollers = triggers.Where(o => o.HasProperty(
        "Triggers Room Transition",
        "true",
        ObjecttypesByName)).ToArray();

      if (fullScreenScrollers.Any())
      {
        yield return CreateCameraModifier(
          fullScreenScrollers,
          bounds,
          "Full Screen Scroller");
      }

      var cameraModifiers = triggers.Except(fullScreenScrollers).ToArray();

      if (cameraModifiers.Any())
      {
        yield return CreateCameraModifier(
            cameraModifiers,
            bounds,
            "Camera Modifier");
      }
    }

    private GameObject CreateCameraModifier(
      Object[] triggers,
      Object boundsObject,
      string prefabName)
    {
      var boundsPropertyInfos = triggers
        .Where(t => t.PolyLine == null)
        .Select(t => new CameraModifierInstantiationArguments.BoundsPropertyInfo
          {
            Bounds = t.GetBounds(),
            Properties = t.GetProperties(ObjecttypesByName)
          })
        .ToArray();

      var asset = LoadPrefabAsset(prefabName);

      return CreateInstantiableObject(
       asset,
       prefabName,
       new CameraModifierInstantiationArguments
       {
         BoundsPropertyInfos = boundsPropertyInfos,
         Bounds = boundsObject.GetBounds()
       });
    }
  }
}
