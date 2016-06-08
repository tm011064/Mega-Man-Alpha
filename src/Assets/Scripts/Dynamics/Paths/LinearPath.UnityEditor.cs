#if UNITY_EDITOR

using UnityEngine;

public partial class LinearPath : SpawnBucketItemBehaviour
{
  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      Gizmos.color = OutlineGizmoColor;

      for (var i = 1; i < Nodes.Count; i++)
      {
        Gizmos.DrawLine(
          gameObject.transform.TransformPoint(Nodes[i - 1]),
          gameObject.transform.TransformPoint(Nodes[i]));
      }
    }
  }
}

#endif
