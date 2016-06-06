﻿#if UNITY_EDITOR
using UnityEngine;

public partial class EnemySpawnManager : SpawnBucketItemBehaviour
{
  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      if (EnemyToSpawn != null)
      {
        var boxCollider2D = EnemyToSpawn.GetComponent<BoxCollider2D>();

        if (boxCollider2D != null)
        {
          _gizmoCenter = boxCollider2D.offset;

          _gizmoExtents = boxCollider2D.size / 2;
        }
      }

      GizmoUtility.DrawBoundingBox(transform.TransformPoint(_gizmoCenter), _gizmoExtents, OutlineGizmoColor);
    }
  }
}
#endif