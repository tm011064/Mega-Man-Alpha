#if UNITY_EDITOR
using UnityEngine;

public partial class CameraModifier : MonoBehaviour
{
  public ImportCameraSettings ImportCameraSettings;

  public void ImportSettings()
  {
    if (ImportCameraSettings.ImportSource == null)
    {
      Debug.LogWarning("Unable to import settings because no object was selected");

      return;
    }

    var multiWayCameraModifier = ImportCameraSettings.ImportSource.GetComponent<MultiWayCameraModifier>();

    var cameraModifier = ImportCameraSettings.ImportSource.GetComponent<CameraModifier>();

    if (multiWayCameraModifier != null)
    {
      switch (ImportCameraSettings.ImportCameraSettingsMode)
      {
        case ImportCameraSettingsMode.FromGreenToGreen:
        case ImportCameraSettingsMode.FromGreenToRed:

          VerticalLockSettings = multiWayCameraModifier.GreenCameraModificationSettings.VerticalLockSettings.Clone();
          HorizontalLockSettings = multiWayCameraModifier.GreenCameraModificationSettings.HorizontalLockSettings.Clone();
          ZoomSettings = multiWayCameraModifier.GreenCameraModificationSettings.ZoomSettings.Clone();
          SmoothDampMoveSettings = multiWayCameraModifier.GreenCameraModificationSettings.SmoothDampMoveSettings.Clone();
          Offset = multiWayCameraModifier.GreenCameraModificationSettings.Offset;
          VerticalCameraFollowMode = multiWayCameraModifier.GreenCameraModificationSettings.VerticalCameraFollowMode;
          HorizontalOffsetDeltaMovementFactor = multiWayCameraModifier.GreenCameraModificationSettings.HorizontalOffsetDeltaMovementFactor;

          Debug.Log("Successfully imported green settings from " + ImportCameraSettings.ImportSource.name + " and applied them to greenCameraModificationSettings");

          break;

        case ImportCameraSettingsMode.FromRedToGreen:
        case ImportCameraSettingsMode.FromRedToRed:

          VerticalLockSettings = multiWayCameraModifier.RedCameraModificationSettings.VerticalLockSettings.Clone();
          HorizontalLockSettings = multiWayCameraModifier.RedCameraModificationSettings.HorizontalLockSettings.Clone();
          ZoomSettings = multiWayCameraModifier.RedCameraModificationSettings.ZoomSettings.Clone();
          SmoothDampMoveSettings = multiWayCameraModifier.RedCameraModificationSettings.SmoothDampMoveSettings.Clone();
          Offset = multiWayCameraModifier.RedCameraModificationSettings.Offset;
          VerticalCameraFollowMode = multiWayCameraModifier.RedCameraModificationSettings.VerticalCameraFollowMode;
          HorizontalOffsetDeltaMovementFactor = multiWayCameraModifier.RedCameraModificationSettings.HorizontalOffsetDeltaMovementFactor;

          Debug.Log("Successfully imported red settings from " + ImportCameraSettings.ImportSource.name + " and applied them to redCameraModificationSettings");

          break;

        case ImportCameraSettingsMode.FromModifierToGreen:
        case ImportCameraSettingsMode.FromModifierToRed:

          Debug.LogError("Unable to import modifer settings because the source is of type MultiWayCameraModifier");

          break;
      }
    }
    else if (cameraModifier != null)
    {
      VerticalLockSettings = cameraModifier.VerticalLockSettings.Clone();
      HorizontalLockSettings = cameraModifier.HorizontalLockSettings.Clone();
      ZoomSettings = cameraModifier.ZoomSettings.Clone();
      SmoothDampMoveSettings = cameraModifier.SmoothDampMoveSettings.Clone();
      Offset = cameraModifier.Offset;
      VerticalCameraFollowMode = cameraModifier.VerticalCameraFollowMode;
      HorizontalOffsetDeltaMovementFactor = cameraModifier.HorizontalOffsetDeltaMovementFactor;
    }
    else
    {
      Debug.LogError("Unable to import settings because object does not contain MultiWayCameraModifier nor CameraModifier component.");

      return;
    }
  }
}
#endif
