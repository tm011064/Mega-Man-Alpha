#if UNITY_EDITOR
using UnityEngine;

public partial class WheelEllipse : SpawnBucketItemBehaviour
{
  public Color OutlineGizmoColor = Color.white;

  public bool ShowGizmoOutline = true;

  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      if (FloatingAttachedPlatform != null)
      {
        var boxCollider = FloatingAttachedPlatform.GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
          _gizmoCenter = Vector2.zero;

          _gizmoExtents = new Vector3(
            Width + boxCollider.size.x / 2,
            Height + boxCollider.size.y / 2);
        }
      }

      GizmoUtility.DrawBoundingBox(transform.TransformPoint(_gizmoCenter), _gizmoExtents, OutlineGizmoColor);
    }
  }
}
#endif