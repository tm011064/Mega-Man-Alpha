using System;
using UnityEngine;

public static class TransformExtensions
{
  public static GameObject GetChildGameObject(this Transform self, string name)
  {
    var child = self.FindChild(name);

    Logger.Assert(
      child != null,
      self.gameObject.name + " is expected to have a '" + name + "' child object");

    return child.gameObject;
  }

  public static void ForEachChildComponent<T>(this Transform self, Action<T> action)
  {
    // note: for some reason GetComponent(s)InChildren<T>() crashes here
    for (var i = 0; i < self.childCount; i++)
    {
      var child = self.GetChild(i);

      var component = child.GetComponent<T>();

      if (component != null)
      {
        action(component);
      }
    }
  }
}
