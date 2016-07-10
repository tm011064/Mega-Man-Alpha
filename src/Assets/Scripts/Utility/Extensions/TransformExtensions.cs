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
}
