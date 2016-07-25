#if UNITY_EDITOR

using UnityEngine;

public partial class FullScreenScroller : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments = null)
  {
    if (arguments != null)
    {
      var boxCollider = GetComponent<BoxCollider2D>();

      if (boxCollider == null)
      {
        throw new MissingComponentException("BoxCollider2D expected");
      }

      transform.position = arguments.Bounds.center;

      boxCollider.size = arguments.Bounds.size;
    }
  }
}

#endif