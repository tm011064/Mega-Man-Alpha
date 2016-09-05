using UnityEngine;

public class CameraMovementSettings
{
  public VerticalLockSettings VerticalLockSettings;

  public HorizontalLockSettings HorizontalLockSettings;

  public ZoomSettings ZoomSettings;

  public SmoothDampMoveSettings SmoothDampMoveSettings;

  public Vector2 Offset;

  public float HorizontalOffsetDeltaMovementFactor;

  public VerticalCameraFollowMode VerticalCameraFollowMode;

  public CameraMovementSettings(
    VerticalLockSettings verticalLockSettings,
    HorizontalLockSettings horizontalLockSettings,
    ZoomSettings zoomSettings,
    SmoothDampMoveSettings smoothDampMoveSettings,
    Vector2 offset,
    VerticalCameraFollowMode verticalCameraFollowMode,
    float horizontalOffsetDeltaMovementFactor)
  {
    HorizontalLockSettings = horizontalLockSettings;
    VerticalLockSettings = verticalLockSettings;
    Offset = offset;
    ZoomSettings = zoomSettings;
    SmoothDampMoveSettings = smoothDampMoveSettings;
    VerticalCameraFollowMode = verticalCameraFollowMode;
    HorizontalOffsetDeltaMovementFactor = horizontalOffsetDeltaMovementFactor;
  }

  public override bool Equals(object obj)
  {
    return obj != null
      && GetHashCode() == obj.GetHashCode();
  }

  public override int GetHashCode()
  {
    unchecked
    {
      int hash = 17;

      hash = hash * 23 + VerticalLockSettings.GetHashCode();
      hash = hash * 23 + HorizontalLockSettings.GetHashCode();
      hash = hash * 23 + ZoomSettings.GetHashCode();
      hash = hash * 23 + SmoothDampMoveSettings.GetHashCode();
      hash = hash * 23 + Offset.GetHashCode();
      hash = hash * 23 + HorizontalOffsetDeltaMovementFactor.GetHashCode();
      hash = hash * 23 + VerticalCameraFollowMode.GetHashCode();

      return hash;
    }
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }

  public bool Contains(Vector2 vector)
  {
    if (!HasBoundaries())
    {
      return false;
    }

    var bounds = GetBounds();

    return bounds.Contains(vector);
  }

  private Bounds GetBounds()
  {
    var centerX =
      HorizontalLockSettings.LeftHorizontalLockPosition
      + (HorizontalLockSettings.RightHorizontalLockPosition - HorizontalLockSettings.LeftHorizontalLockPosition) * .5f;

    var centerY = VerticalLockSettings.EnableDefaultVerticalLockPosition
      ? VerticalLockSettings.DefaultVerticalLockPosition
      : VerticalLockSettings.BottomVerticalLockPosition
        + (VerticalLockSettings.TopVerticalLockPosition - VerticalLockSettings.BottomVerticalLockPosition) * .5f;

    var width = HorizontalLockSettings.RightHorizontalLockPosition - HorizontalLockSettings.LeftHorizontalLockPosition;

    var height = GetHeight();

    return new Bounds(
      new Vector3(centerX, centerY),
      new Vector3(width, height));
  }

  private float GetHeight()
  {
    if (VerticalLockSettings.EnableDefaultVerticalLockPosition)
    {
      var cameraController = Camera.main.GetComponent<CameraController>();

      return cameraController.TargetScreenSize.y;
    }

    return VerticalLockSettings.TopVerticalLockPosition - VerticalLockSettings.BottomVerticalLockPosition;
  }

  private bool HasBoundaries()
  {
    return HorizontalLockSettings.Enabled
      && HorizontalLockSettings.EnableLeftHorizontalLock
      && HorizontalLockSettings.EnableRightHorizontalLock
      && VerticalLockSettings.Enabled
      && (VerticalLockSettings.EnableDefaultVerticalLockPosition
        || (VerticalLockSettings.EnableTopVerticalLock && VerticalLockSettings.EnableBottomVerticalLock));
  }
}
