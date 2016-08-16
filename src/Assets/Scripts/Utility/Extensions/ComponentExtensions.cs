using UnityEngine;

public static class ComponentExtensions
{
  public static TComponent GetComponentOrThrow<TComponent>(this Component self)
  {
    var component = self.GetComponent<TComponent>();

    if (component == null)
    {
      throw new MissingComponentException("Component of type '" + typeof(TComponent).ToString() + "' missing from game object '" + self.gameObject.name + "'");
    }

    return component;
  }
}
