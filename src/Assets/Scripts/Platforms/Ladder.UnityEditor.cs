#if UNITY_EDITOR

using UnityEngine;

public partial class Ladder : IInstantiable<InstantiationArguments>
{
  public void Instantiate(InstantiationArguments arguments)
  {
    if (arguments != null)
    {
      Size = new Vector2(
        arguments.Properties.GetInt("Width"),
        arguments.Bounds.size.y);

      transform.position = arguments.Bounds.center;
    }
  }
}

#endif
