#if UNITY_EDITOR

using UnityEngine;

public partial class Ladder : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments)
  {
    if (arguments != null)
    {
      Size = new Vector2(
        arguments.GetInt("Width"),
        arguments.Bounds.size.y);

      transform.position = arguments.Bounds.center;
    }
  }
}

#endif
