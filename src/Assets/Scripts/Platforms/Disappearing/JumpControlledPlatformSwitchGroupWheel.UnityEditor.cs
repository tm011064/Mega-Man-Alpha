#if UNITY_EDITOR

using UnityEngine;

public partial class JumpControlledPlatformSwitchGroupWheel : SpawnBucketItemBehaviour
{
  public Color OutlineGizmoColor = Color.white;

  public bool ShowGizmoOutline = true;

  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline && PlatformGroups.Count > 0)
    {
      BoxCollider2D boxCollider = null;

      for (var i = 0; i < PlatformGroups.Count; i++)
      {
        if (PlatformGroups[i].EnabledGameObject != null)
        {
          boxCollider = PlatformGroups[i].EnabledGameObject.GetComponent<BoxCollider2D>();

          break;
        }

        if (PlatformGroups[i].DisabledGameObject != null)
        {
          boxCollider = PlatformGroups[i].DisabledGameObject.GetComponent<BoxCollider2D>();

          break;
        }
      }

      if (boxCollider != null)
      {
        _gizmoCenter = Vector2.zero;

        _gizmoExtents = new Vector3(
          Width + boxCollider.size.x / 2,
          Height + boxCollider.size.y / 2);
      }

      GizmoUtility.DrawBoundingBox(transform.TransformPoint(_gizmoCenter), _gizmoExtents, OutlineGizmoColor);
    }
  }
}

#endif