#if UNITY_EDITOR

using UnityEngine;

public partial class EdgeColliderTriggerEnterBehaviour : IInstantiable
{
  public void Instantiate(InstantiationArguments arguments)
  {
    foreach (var line in arguments.Lines)
    {
      var edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

      edgeCollider.isTrigger = true;
      edgeCollider.points = line.ToVectors();
    }

    transform.position = arguments.Bounds.center;
  }
}

#endif
