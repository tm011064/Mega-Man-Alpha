#if UNITY_EDITOR
using UnityEngine;

public partial class CameraModifier : IInstantiable
{
  public ImportCameraSettings ImportCameraSettings;

  public void Instantiate(InstantiationArguments arguments)
  {
    var cameraController = Camera.main.GetComponentOrThrow<CameraController>();

    VerticalLockSettings = CreateVerticalLockSettings(arguments.Bounds, cameraController);
    HorizontalLockSettings = CreateHorizontalLockSettings(arguments.Bounds, cameraController);

    transform.position = new Vector2(
      arguments.Vectors[0].x + (arguments.Vectors[1].x - arguments.Vectors[0].x) / 2,
      arguments.Vectors[0].y + (arguments.Vectors[1].y - arguments.Vectors[0].y) / 2);

    var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();

    boxCollider.size = new Vector2(1, Mathf.Abs(arguments.Vectors[1].y - arguments.Vectors[0].y));
  }

  private VerticalLockSettings CreateVerticalLockSettings(Bounds bounds, CameraController cameraController)
  {
    var verticalLockSettings = new VerticalLockSettings
    {
      Enabled = true,
      EnableDefaultVerticalLockPosition = false,
      DefaultVerticalLockPosition = 0f,
      EnableTopVerticalLock = true,
      EnableBottomVerticalLock = true,
      TopVerticalLockPosition = bounds.max.y,
      BottomVerticalLockPosition = bounds.min.y
    };

    verticalLockSettings.TopBoundary =
      verticalLockSettings.TopVerticalLockPosition
      - cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.BottomBoundary =
      verticalLockSettings.BottomVerticalLockPosition
      + cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.TranslatedVerticalLockPosition =
      verticalLockSettings.DefaultVerticalLockPosition;

    return verticalLockSettings;
  }

  private HorizontalLockSettings CreateHorizontalLockSettings(Bounds bounds, CameraController cameraController)
  {
    var horizontalLockSettings = new HorizontalLockSettings
    {
      Enabled = true,
      EnableLeftHorizontalLock = true,
      EnableRightHorizontalLock = true,
      LeftHorizontalLockPosition = bounds.min.x,
      RightHorizontalLockPosition = bounds.max.x
    };

    horizontalLockSettings.LeftBoundary =
      horizontalLockSettings.LeftHorizontalLockPosition
      + cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    horizontalLockSettings.RightBoundary =
      horizontalLockSettings.RightHorizontalLockPosition
      - cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    return horizontalLockSettings;
  }

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
