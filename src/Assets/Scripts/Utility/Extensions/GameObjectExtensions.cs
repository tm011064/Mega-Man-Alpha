using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
  public static void AttachChild(this GameObject self, GameObject child)
  {
    child.transform.parent = self.transform;
  }

  //public static void AttachChildren(this GameObject self, params GameObject[] children)
  //{
  //  foreach (var child in children)
  //  {
  //    self.AttachChild(child);
  //  }
  //}

  public static void AttachChildren(this GameObject self, IEnumerable<GameObject> children)
  {
    foreach (var child in children)
    {
      self.AttachChild(child);
    }
  }
}
