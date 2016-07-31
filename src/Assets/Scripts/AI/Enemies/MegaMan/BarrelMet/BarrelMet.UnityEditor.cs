#if UNITY_EDITOR

using UnityEngine;

public partial class BarrelMet : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments)
  {
    if (arguments.IsFlippedHorizontally)
    {
      transform.localScale = new Vector3(-1, 1, 1);
    }
  }
}

#endif
