using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
  public static void AttachChild(this GameObject self, GameObject child)
  {
    child.transform.parent = self.transform;
  }

  public static TComponent GetComponentOrThrow<TComponent>(this GameObject self)
  {
    var component = self.GetComponent<TComponent>();

    if (component == null)
    {
      throw new MissingComponentException("Component of type '" + typeof(TComponent).ToString() + "' missing from game object '" + self.name + "'");
    }

    return component;
  }

  public static void AttachChildren(this GameObject self, IEnumerable<GameObject> children)
  {
    foreach (var child in children)
    {
      self.AttachChild(child);
    }
  }
}
