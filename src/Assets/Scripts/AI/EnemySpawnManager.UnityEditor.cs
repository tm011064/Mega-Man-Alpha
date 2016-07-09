#if UNITY_EDITOR
using UnityEngine;

public partial class EnemySpawnManager : SpawnBucketItemBehaviour
{
  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;

  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      if (EnemyToSpawnPrefab != null)
      {
        var boxCollider2D = EnemyToSpawnPrefab.GetComponent<BoxCollider2D>();

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