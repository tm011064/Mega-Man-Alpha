using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public static class TiledXmlExtensions
  {
    public static Dictionary<string, string> GetProperties(this Object obj, Dictionary<string, Objecttype> objecttypesByName)
    {
      var properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

      Objecttype objecttype;

      if (objecttypesByName.TryGetValue(obj.Type, out objecttype))
      {
        foreach (var property in objecttype.Properties)
        {
          properties[property.Name] = property.Default;
        }
      }

      if (obj.Properties != null)
      {
        foreach (var property in obj.Properties.Property)
        {
          properties[property.Name] = property.Value;
        }
      }

      return properties;
    }

    public static IEnumerable<Object> ForEachObjectWithProperty(
      this Map map,
      string propertyName,
      Dictionary<string, Objecttype> objecttypesByName)
    {
      return map
        .Objectgroup
        .SelectMany(og => og
          .Object
          .Where(o => o.HasProperty(propertyName, objecttypesByName)));
    }

    public static bool HasProperty(this Object obj, string propertyName, Dictionary<string, Objecttype> objecttypesByName)
    {
      return GetProperties(obj, objecttypesByName)
        .ContainsKey(propertyName);
    }

    public static Bounds GetBounds(this Object obj)
    {
      if (obj.IsImage())
      {
        return new Bounds(
          new Vector2(obj.X + obj.Width / 2, -obj.Y),
          new Vector2(obj.Width, obj.Height));
      }

      return new Bounds(
          new Vector2(obj.X + obj.Width / 2, -(obj.Y + obj.Height / 2)),
          new Vector2(obj.Width, obj.Height));
    }

    public static bool IsImage(this Object obj)
    {
      return obj.Gid.HasValue;
    }

    public static bool IsCollider(this Object obj)
    {
      return !obj.Gid.HasValue;
    }

    public static IEnumerable<Layer> ForEachLayerWithProperty(this Map map, string propertyName, string propertyValue)
    {
      return map
        .Layers
        .Where(c => c.Properties != null && c.Properties.Property.Any(
          p => string.Compare(p.Name.Trim(), propertyName, true) == 0
            && string.Compare(p.Value.Trim(), propertyValue, true) == 0));
    }

    public static IEnumerable<Layer> ForEachLayerWithPropertyName(this Map map, string propertyName)
    {
      return map
        .Layers
        .Where(c => c.Properties != null
          && c.Properties.Property.Any(
            p => string.Compare(p.Name.Trim(), propertyName, true) == 0));
    }

    public static void Execute(this IEnumerable<Layer> layers, Action<Layer> action)
    {
      foreach (var layer in layers)
      {
        action(layer);
      }
    }

    public static IEnumerable<T> Get<T>(this IEnumerable<Layer> layers, Func<Layer, T> func)
    {
      foreach (var layer in layers)
      {
        yield return func(layer);
      }
    }

    public static IEnumerable<T> Get<T>(this IEnumerable<Object> objects, Func<Object, T> func)
    {
      foreach (var obj in objects)
      {
        yield return func(obj);
      }
    }
  }
}
