using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public partial class FullScreenScroller : MonoBehaviour, ISceneResetable
{
  public ZoomSettings ZoomSettings;

  public SmoothDampMoveSettings SmoothDampMoveSettings;

  public FullScreenScrollSettings FullScreenScrollSettings;

  [Tooltip("By default, all lock positions are relative to this game object's parent object. You can overwrite this by setting this gameobject as parent")]
  public GameObject RelativePositioningParentObject;

  [Tooltip("The (x, y) offset of the camera. This can be used when default vertical locking is disabled and you want the player to be below, above, right or left of the screen center.")]
  public Vector2 Offset;

  [Tooltip("The dimensions of the camera boundaries")]
  public Vector2 Size;

  public bool MustBeOnLadderToEnter;

  public float HorizontalOffsetDeltaMovementFactor = 40f;

  public VerticalCameraFollowMode VerticalCameraFollowMode;

  private GameObject _parent;

  private CameraController _cameraController;

  private bool _skipEnter;

  private int _animationShortNameHash;

  private Checkpoint _checkpoint;

  private CameraMovementSettings _cameraMovementSettings;

  void Awake()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();

    OnSceneReset();

    var enterTrigger = GetComponentInChildren<ITriggerEnterExit>();

    if (enterTrigger != null)
    {
      enterTrigger.Entered += (_, e) => OnEnter(e.SourceCollider);
      enterTrigger.Exited += (_, e) => _cameraController.OnCameraModifierExit(_cameraMovementSettings);
    }

    _checkpoint = GetComponentInChildren<Checkpoint>();

    _cameraMovementSettings = CreateCameraMovementSettings();
  }

  public void OnSceneReset()
  {
    var bounds = new Bounds(transform.position, Size);

    _skipEnter = bounds.Contains(GameManager.Instance.Player.transform.position);
  }

  private CameraMovementSettings CreateCameraMovementSettings()
  {
    var horizontalLockSettings = CreateHorizontalLockSettings();

    var verticalLockSettings = CreateVerticalLockSettings();

    return new CameraMovementSettings(
      verticalLockSettings,
      horizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      Offset,
      VerticalCameraFollowMode,
      HorizontalOffsetDeltaMovementFactor);
  }

  private void StartScroll(Collider2D collider)
  {
    _cameraController.OnCameraModifierEnter(_cameraMovementSettings);

    // the order here is important. First we want to set the camera movement settings, then we can create
    // the scroll transform action.
    var targetPosition = _cameraController.CalculateTargetPosition();

    PushControlHandlers(targetPosition);

    var scrollTransformationAction = new TranslateTransformAction(
      targetPosition,
      FullScreenScrollSettings.TransitionTime,
      EasingType.Linear,
      GameManager.Instance.Easing);

    _cameraController.EnqueueScrollAction(scrollTransformationAction);
  }

  private void PushControlHandlers(Vector3 targetPosition)
  {
    Vector3? playerTranslationVector = GetPlayerTranslationVector(targetPosition);

    var freezeControlHandlers = new Queue<FreezePlayerControlHandler>(GetScrollControlHandlers(playerTranslationVector));

    GameManager.Instance.Player.ExchangeActiveControlHandler(
      freezeControlHandlers.Dequeue());

    while (freezeControlHandlers.Any())
    {
      GameManager.Instance.Player.PushControlHandler(
        freezeControlHandlers.Dequeue());
    }
  }

  private Vector3? GetPlayerTranslationVector(Vector3 targetPosition)
  {
    if (FullScreenScrollSettings.PlayerTranslationDistance == 0f)
    {
      return null;
    }

    var currentCameraPosition = _cameraController.gameObject.transform.position;

    var directionVector = targetPosition - currentCameraPosition;

    return directionVector.normalized * FullScreenScrollSettings.PlayerTranslationDistance;
  }

  private IEnumerable<FreezePlayerControlHandler> GetScrollControlHandlers(Vector3? playerTranslationVector)
  {
    if (FullScreenScrollSettings.EndScrollFreezeTime > 0f)
    {
      yield return new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          FullScreenScrollSettings.EndScrollFreezeTime,
          _animationShortNameHash);
    }

    yield return new FreezePlayerControlHandler(
        GameManager.Instance.Player,
        FullScreenScrollSettings.TransitionTime,
        _animationShortNameHash,
        playerTranslationVector,
        FullScreenScrollSettings.PlayerTranslationEasingType);
  }

  private void OnEnter(Collider2D collider)
  {
    UpdatePlayerSpawnLocation();

    if (_skipEnter)
    {
      _skipEnter = false;

      _cameraController.OnCameraModifierEnter(_cameraMovementSettings);

      _cameraController.MoveCameraToTargetPosition(GameManager.Instance.Player.transform.position);

      return;
    }

    if (_cameraController.IsCurrentCameraMovementSettings(_cameraMovementSettings))
    {
      return;
    }

    var currentAnimatorStateInfo = GameManager.Instance.Player.Animator.GetCurrentAnimatorStateInfo(0);

    _animationShortNameHash = currentAnimatorStateInfo.shortNameHash;

    if (FullScreenScrollSettings.StartScrollFreezeTime > 0f)
    {
      var delay = TimeSpan.FromSeconds(FullScreenScrollSettings.StartScrollFreezeTime);

      GameManager.Instance.Player.PushControlHandler(
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          FullScreenScrollSettings.StartScrollFreezeTime + (float)delay.TotalSeconds,
          _animationShortNameHash));

      this.Invoke(delay, () => StartScroll(collider));
    }
    else
    {
      StartScroll(collider);
    }
  }

  private void UpdatePlayerSpawnLocation()
  {
    if (_checkpoint != null)
    {
      GameManager.Instance.Player.SpawnLocation = _checkpoint.transform.position;
    }
  }

  private VerticalLockSettings CreateVerticalLockSettings()
  {
    var verticalLockSettings = new VerticalLockSettings
    {
      Enabled = true,
      EnableDefaultVerticalLockPosition = false,
      DefaultVerticalLockPosition = 0f,
      EnableTopVerticalLock = true,
      EnableBottomVerticalLock = true,
      TopVerticalLockPosition = transform.position.y + Size.y * .5f,
      BottomVerticalLockPosition = transform.position.y - Size.y * .5f
    };

    verticalLockSettings.TopBoundary =
      verticalLockSettings.TopVerticalLockPosition
      - _cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.BottomBoundary =
      verticalLockSettings.BottomVerticalLockPosition
      + _cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    return verticalLockSettings;
  }

  private HorizontalLockSettings CreateHorizontalLockSettings()
  {
    var horizontalLockSettings = new HorizontalLockSettings
    {
      Enabled = true,
      EnableLeftHorizontalLock = true,
      EnableRightHorizontalLock = true,
      LeftHorizontalLockPosition = transform.position.x - Size.x * .5f,
      RightHorizontalLockPosition = transform.position.x + Size.x * .5f
    };

    horizontalLockSettings.LeftBoundary =
      horizontalLockSettings.LeftHorizontalLockPosition
      + _cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    horizontalLockSettings.RightBoundary =
      horizontalLockSettings.RightHorizontalLockPosition
      - _cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    return horizontalLockSettings;
  }
}