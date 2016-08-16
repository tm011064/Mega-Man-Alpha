#if UNITY_EDITOR

using UnityEngine;

public partial class EdgeColliderTriggerEnterBehaviour : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments)
  {
    var edgeCollider = this.GetComponentOrThrow<EdgeCollider2D>();

    edgeCollider.points = arguments.Vectors;

    transform.position = arguments.Bounds.center;
  }
}

#endif
