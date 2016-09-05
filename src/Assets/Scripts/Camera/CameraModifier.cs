using System;
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

  public bool MustBeOnLadderToEnter;

  private CameraController _cameraController;

  private CameraMovementSettings _cameraMovementSettings;

  void Awake()
  {
    var triggerEnterBehaviours = GetComponentsInChildren<ITriggerEnterExit>();

    foreach (var triggerEnterBehaviour in triggerEnterBehaviours)
    {
      triggerEnterBehaviour.Entered += OnEnterTriggerInvoked;
      triggerEnterBehaviour.Exited += OnExitTriggerInvoked;
    }
  }

  void Start()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();

    _cameraMovementSettings = CreateCameraMovementSettings();

    var playerPosition = GameManager.Instance.Player.transform.position;

    if (_cameraMovementSettings.Contains(playerPosition))
    {
      _cameraController.OnCameraModifierEnter(_cameraMovementSettings);
    }
  }

  private void SetHorizontalBoundaries(HorizontalLockSettings horizontalLockSettings, CameraController cameraController)
  {
    horizontalLockSettings.LeftBoundary =
      horizontalLockSettings.LeftHorizontalLockPosition
      + cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    horizontalLockSettings.RightBoundary =
      horizontalLockSettings.RightHorizontalLockPosition
      - cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;
  }

  private void SetVerticalBoundaries(VerticalLockSettings verticalLockSettings, CameraController cameraController)
  {
    verticalLockSettings.TopBoundary =
      verticalLockSettings.TopVerticalLockPosition
      - cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.BottomBoundary =
      verticalLockSettings.BottomVerticalLockPosition
      + cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;
  }

  private CameraMovementSettings CreateCameraMovementSettings()
  {
    if (ZoomSettings.ZoomPercentage == 0f)
    {
      throw new ArgumentOutOfRangeException("Zoom Percentage must not be 0.");
    }

    SetVerticalBoundaries(VerticalLockSettings, _cameraController);
    SetHorizontalBoundaries(HorizontalLockSettings, _cameraController);

    return new CameraMovementSettings(
      VerticalLockSettings,
      HorizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      Offset,
      VerticalCameraFollowMode,
      HorizontalOffsetDeltaMovementFactor);
  }

  void OnEnterTriggerInvoked(object sender, TriggerEnterExitEventArgs e)
  {
    if (MustBeOnLadderToEnter
      && (GameManager.Instance.Player.PlayerState & PlayerState.ClimbingLadder) == 0)
    {
      return;
    }

    _cameraController.OnCameraModifierEnter(_cameraMovementSettings);
  }

  void OnExitTriggerInvoked(object sender, TriggerEnterExitEventArgs e)
  {
    _cameraController.OnCameraModifierExit(_cameraMovementSettings);
  }
}
