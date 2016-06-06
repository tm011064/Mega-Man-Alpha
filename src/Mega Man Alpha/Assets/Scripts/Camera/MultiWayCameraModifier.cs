using System;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public partial class MultiWayCameraModifier : MonoBehaviour
{
  public MultiWayCameraModificationSetting RedCameraModificationSettings = new MultiWayCameraModificationSetting();

  public MultiWayCameraModificationSetting GreenCameraModificationSettings = new MultiWayCameraModificationSetting();

  public float EdgeColliderLength = 256f;

  public float EdgeColliderAngle = 0f;

  [Tooltip("All lock positions are relative to this object.")]
  public GameObject ParentPositionObject;

  private CameraController _cameraController;

  private EdgeCollider2D _edgeCollider;

  private MultiWayCameraModificationSetting _lastMultiWayCameraModificationSetting;

  void Start()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();

    _edgeCollider = GetComponent<EdgeCollider2D>();
  }

  private bool IsLeft(Vector2 lineFrom, Vector2 lineTo, Vector2 point)
  {
    return (
              (lineTo.x - lineFrom.x) * (point.y - lineFrom.y)
            - (lineTo.y - lineFrom.y) * (point.x - lineFrom.x)
           ) > 0;
  }

  private MultiWayCameraModificationSetting CloneAndTranslaceCameraModificationSetting(
    MultiWayCameraModificationSetting source,
    CameraController cameraController)
  {
    var clone = source.Clone();

    var transformPoint = ParentPositionObject.transform.TransformPoint(Vector3.zero);

    if (clone.VerticalLockSettings.Enabled)
    {
      if (clone.VerticalLockSettings.EnableTopVerticalLock)
      {
        clone.VerticalLockSettings.TopBoundary =
          transformPoint.y
          + clone.VerticalLockSettings.TopVerticalLockPosition
          - cameraController.TargetScreenSize.y * .5f / clone.ZoomSettings.zoomPercentage;
      }

      if (clone.VerticalLockSettings.EnableBottomVerticalLock)
      {
        clone.VerticalLockSettings.BottomBoundary =
          transformPoint.y
          + clone.VerticalLockSettings.BottomVerticalLockPosition
          + cameraController.TargetScreenSize.y * .5f / clone.ZoomSettings.zoomPercentage;
      }
    }
    if (clone.HorizontalLockSettings.Enabled)
    {
      if (clone.HorizontalLockSettings.EnableLeftHorizontalLock)
      {
        clone.HorizontalLockSettings.LeftBoundary =
          transformPoint.x
          + clone.HorizontalLockSettings.LeftHorizontalLockPosition
          + cameraController.TargetScreenSize.x * .5f / clone.ZoomSettings.zoomPercentage;
      }

      if (clone.HorizontalLockSettings.EnableRightHorizontalLock)
      {
        clone.HorizontalLockSettings.RightBoundary =
          transformPoint.x
          + clone.HorizontalLockSettings.RightHorizontalLockPosition
          - cameraController.TargetScreenSize.x * .5f / clone.ZoomSettings.zoomPercentage;
      }
    }

    clone.VerticalLockSettings.TranslatedVerticalLockPosition =
      transformPoint.y + clone.VerticalLockSettings.DefaultVerticalLockPosition;

    return clone;
  }

  private void SetCameraModificationSettings(MultiWayCameraModificationSetting source)
  {
    if (source.ZoomSettings.zoomPercentage == 0f)
    {
      throw new ArgumentOutOfRangeException("Zoom Percentage must not be 0.");
    }

    var clone = CloneAndTranslaceCameraModificationSetting(source, _cameraController);

    var cameraMovementSettings = new CameraMovementSettings(
      clone.VerticalLockSettings,
      clone.HorizontalLockSettings,
      clone.ZoomSettings,
      clone.SmoothDampMoveSettings,
      clone.Offset,
      clone.VerticalCameraFollowMode,
      clone.HorizontalOffsetDeltaMovementFactor);

    var cameraController = Camera.main.GetComponent<CameraController>();

    cameraController.SetCameraMovementSettings(cameraMovementSettings);

    _lastMultiWayCameraModificationSetting = source;

    Logger.Info("Applied " + (source == RedCameraModificationSettings ? "red" : "green") + " camera setting for camera modifier " + name);
  }

  void OnTriggerExit2D(Collider2D col)
  {
    var isLeft = IsLeft(
      _edgeCollider.points[0],
      _edgeCollider.points[1],
      transform.InverseTransformPoint(col.gameObject.transform.position));

    // left is red
    if (isLeft)
    {
      // we exit from to the left, meaning that we came from green and go to red
      if (_lastMultiWayCameraModificationSetting != RedCameraModificationSettings)
      {
        SetCameraModificationSettings(RedCameraModificationSettings);
      }
    }
    else
    {
      // we exit from to the right, meaning that we came from red and go to green
      if (_lastMultiWayCameraModificationSetting != GreenCameraModificationSettings)
      {
        SetCameraModificationSettings(GreenCameraModificationSettings);
      }
    }
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    var isLeft = IsLeft(
      _edgeCollider.points[0],
      _edgeCollider.points[1],
      transform.InverseTransformPoint(col.gameObject.transform.position));

    // left is red
    if (isLeft)
    {
      // we enter from the left, meaning that we came from red and go to green
      SetCameraModificationSettings(GreenCameraModificationSettings);
    }
    else
    {
      // we enter from the right, meaning that we came from gree and go to red
      SetCameraModificationSettings(RedCameraModificationSettings);
    }
  }
}
