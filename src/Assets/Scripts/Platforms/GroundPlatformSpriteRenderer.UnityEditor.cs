#if UNITY_EDITOR

using UnityEngine;

public partial class GroundPlatformSpriteRenderer : BasePlatform
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

    if ((int)_physicsCollider.bounds.extents.x != Width
      || (int)_physicsCollider.bounds.extents.y != Height)
    {
      _physicsCollider.size = new Vector2(Width, Height);
    }

    if ((int)_visibilityCollider.bounds.extents.x != Width
      || (int)_visibilityCollider.bounds.extents.y != Height)
    {
      _visibilityCollider.size = new Vector2(Width + Screen.width / 2, Height + Screen.height / 2); // add some padding
    }

    if (ShowCameraGizmoOutline)
    {
      GizmoUtility.DrawBoundingBox(transform, _visibilityCollider.bounds.extents, OutlineVisibilityMaskGizmoColor);
    }

    if (ShowGizmoOutline)
    {
      GizmoUtility.DrawBoundingBox(transform, _physicsCollider.bounds.extents, OutlineGizmoColor);
    }
  }
}

#endif