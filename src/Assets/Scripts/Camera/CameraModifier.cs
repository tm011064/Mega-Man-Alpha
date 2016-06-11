﻿using System;
using UnityEngine;

public partial class CameraModifier : MonoBehaviour
{
  public VerticalLockSettings VerticalLockSettings;

  public HorizontalLockSettings HorizontalLockSettings;

  public ZoomSettings ZoomSettings;

  public SmoothDampMoveSettings SmoothDampMoveSettings;

  [Tooltip("The (x, y) offset of the camera. This can be used when default vertical locking is disabled and you want the player to be below, above, right or left of the screen center.")]
  public Vector2 Offset;

  public float HorizontalOffsetDeltaMovementFactor = 40f;

  public VerticalCameraFollowMode VerticalCameraFollowMode;

  public Color GizmoColor = Color.magenta;

  [Tooltip("All lock positions are relative to this object.")]
  public GameObject ParentPositionObject;

  private CameraController _cameraController;

  void OnDrawGizmos()
  {
    foreach (var boxCollider in GetComponents<BoxCollider2D>())
    {
      GizmoUtility.DrawBoundingBox(transform.position, boxCollider.bounds.extents, GizmoColor);

      break;
    }
  }

  void Start()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();

    if (ParentPositionObject == null)
    {
      throw new MissingComponentException("parentPositionObject for camera modifier not set");
    }
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    Vector3 transformPoint = ParentPositionObject.transform.TransformPoint(Vector3.zero);

    if (ZoomSettings.ZoomPercentage == 0f)
    {
      throw new ArgumentOutOfRangeException("Zoom Percentage must not be 0.");
    }

    if (VerticalLockSettings.Enabled)
    {
      if (VerticalLockSettings.EnableTopVerticalLock)
      {
        VerticalLockSettings.TopBoundary =
          transformPoint.y
          + VerticalLockSettings.TopVerticalLockPosition
          - _cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;
      }

      if (VerticalLockSettings.EnableBottomVerticalLock)
      {
        VerticalLockSettings.BottomBoundary =
          transformPoint.y
          + VerticalLockSettings.BottomVerticalLockPosition
          + _cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;
      }
    }

    if (HorizontalLockSettings.Enabled)
    {
      if (HorizontalLockSettings.EnableLeftHorizontalLock)
      {
        HorizontalLockSettings.LeftBoundary = transformPoint.x + HorizontalLockSettings.LeftHorizontalLockPosition + _cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;
      }

      if (HorizontalLockSettings.EnableRightHorizontalLock)
      {
        HorizontalLockSettings.RightBoundary = transformPoint.x + HorizontalLockSettings.RightHorizontalLockPosition - _cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;
      }
    }

    VerticalLockSettings.TranslatedVerticalLockPosition =
      transformPoint.y + VerticalLockSettings.DefaultVerticalLockPosition;

    var cameraMovementSettings = new CameraMovementSettings(
      VerticalLockSettings,
      HorizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      Offset,
      VerticalCameraFollowMode,
      HorizontalOffsetDeltaMovementFactor);

    var cameraController = Camera.main.GetComponent<CameraController>();

    cameraController.SetCameraMovementSettings(cameraMovementSettings);
  }
}