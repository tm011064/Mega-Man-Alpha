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

  public override string ToString()
  {
    return string.Format(@"verticalLockSettings {{ {0} }}
horizontalLockSettings {{ {1} }}
zoomSettings {{ {2} }}
smoothDampMoveSettings {{ {3} }}
offset: {4}; verticalCameraFollowMode: {5}",
      VerticalLockSettings,
      HorizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      Offset,
      VerticalCameraFollowMode);
  }

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
}
