#if UNITY_EDITOR

using UnityEngine;

public partial class OneWayPlatformSpriteRenderer : BasePlatform
{
  public Color OutlineGizmoColor = Color.white;

  public Color OutlineVisibilityMaskGizmoColor = Color.magenta;

  public bool ShowGizmoOutline = true;

  public bool ShowCameraGizmoOutline = true;

  void OnDrawGizmos()
  {
    if (_physicsCollider == null || _visibilityCollider == null)
    {
      Awake();
    }

    if (ShowCameraGizmoOutline)
    {
      GizmoUtility.DrawBoundingBox(transform, _visibilityCollider.bounds.extents, OutlineVisibilityMaskGizmoColor);
    }

    if (ShowGizmoOutline)
    {
      GizmoUtility.DrawBoundingBox(transform, new Vector3(Width / 2, Height / 2), OutlineGizmoColor);
    }
  }
}

#endif