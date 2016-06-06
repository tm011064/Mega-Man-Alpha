using System;
using UnityEngine;

[Serializable]
public class MultiWayCameraModificationSetting
{
  public VerticalLockSettings VerticalLockSettings;

  public HorizontalLockSettings HorizontalLockSettings;

  public ZoomSettings ZoomSettings;

  public SmoothDampMoveSettings SmoothDampMoveSettings;

  [Tooltip("The (x, y) offset of the camera. This can be used when default vertical locking is disabled and you want the player to be below, above, right or left of the screen center.")]
  public Vector2 Offset;

  public float HorizontalOffsetDeltaMovementFactor = 40f;

  public VerticalCameraFollowMode VerticalCameraFollowMode;

  public MultiWayCameraModificationSetting Clone()
  {
    var multiWayCameraModificationSetting = new MultiWayCameraModificationSetting();

    multiWayCameraModificationSetting.VerticalLockSettings = VerticalLockSettings.Clone();
    multiWayCameraModificationSetting.HorizontalLockSettings = HorizontalLockSettings.Clone();
    multiWayCameraModificationSetting.ZoomSettings = ZoomSettings.Clone();
    multiWayCameraModificationSetting.SmoothDampMoveSettings = SmoothDampMoveSettings.Clone();
    multiWayCameraModificationSetting.Offset = Offset;
    multiWayCameraModificationSetting.VerticalCameraFollowMode = VerticalCameraFollowMode;
    multiWayCameraModificationSetting.HorizontalOffsetDeltaMovementFactor = HorizontalOffsetDeltaMovementFactor;

    return multiWayCameraModificationSetting;
  }
}
