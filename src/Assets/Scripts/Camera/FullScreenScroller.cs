using System;
using UnityEngine;

public partial class FullScreenScroller : MonoBehaviour, ISceneResetable
{
  public ZoomSettings ZoomSettings;

  public SmoothDampMoveSettings SmoothDampMoveSettings;

  public FullScreenScrollSettings FullScreenScrollSettings;

  [Tooltip("Enables the default vertical lock position. The default position simulates a Super Mario Bros style side scrolling camera which is fixed on the y axis, not reacting to vertical player movement.")]
  public bool EnableDefaultVerticalLockPosition = true;

  [Tooltip("Default is center of the screen.")]
  public float DefaultVerticalLockPosition = 540f;

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

  private VerticalLockSettings _verticalLockSettings;

  private HorizontalLockSettings _horizontalLockSettings;

  private bool _skipEnter;

  private int _animationShortNameHash;

  private Checkpoint _checkpoint;

  private bool _hasEnterTrigger;

  void Awake()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();

    OnSceneReset();

    var enterTrigger = GetComponentInChildren<ITriggerEnterExit>();

    if (enterTrigger != null)
    {
      enterTrigger.Entered += (_, e) => OnEnter(e.SourceCollider);

      _hasEnterTrigger = true;
    }

    _checkpoint = GetComponentInChildren<Checkpoint>();

    _horizontalLockSettings = CreateHorizontalLockSettings();

    _verticalLockSettings = CreateVerticalLockSettings();
  }

  public void OnSceneReset()
  {
    var bounds = new Bounds(transform.position, Size);

    _skipEnter = bounds.Contains(GameManager.Instance.Player.transform.position);
  }

  private void SetCameraMovementSettings(Collider2D collider)
  {
    var cameraMovementSettings = new CameraMovementSettings(
      _verticalLockSettings,
      _horizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      Offset,
      VerticalCameraFollowMode,
      HorizontalOffsetDeltaMovementFactor);

    _cameraController.OnCameraModifierEnter(
      this,
      collider,
      GameManager.Instance.Player.transform.position,
      cameraMovementSettings);
  }

  private void StartScroll(Collider2D collider)
  {
    SetCameraMovementSettings(collider);

    // the order here is important. First we want to set the camera movement settings, then we can create
    // the scroll transform action.
    var targetPosition = _cameraController.CalculateTargetPosition();

    Vector3? playerTranslationVector = null;

    if (FullScreenScrollSettings.PlayerTranslationDistance != 0f)
    {
      var currentCameraPosition = _cameraController.gameObject.transform.position;

      var directionVector = targetPosition - currentCameraPosition;

      playerTranslationVector = directionVector.normalized * FullScreenScrollSettings.PlayerTranslationDistance;
    }

    if (FullScreenScrollSettings.EndScrollFreezeTime > 0f)
    {
      GameManager.Instance.Player.PushControlHandler(
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          FullScreenScrollSettings.EndScrollFreezeTime,
          _animationShortNameHash));
    }

    GameManager.Instance.Player.PushControlHandler(
      new FreezePlayerControlHandler(
        GameManager.Instance.Player,
        FullScreenScrollSettings.TransitionTime,
        _animationShortNameHash,
        playerTranslationVector,
        FullScreenScrollSettings.PlayerTranslationEasingType));

    var scrollTransformationAction = new TranslateTransformAction(
      targetPosition,
      FullScreenScrollSettings.TransitionTime,
      EasingType.Linear,
      GameManager.Instance.Easing);

    _cameraController.EnqueueScrollAction(scrollTransformationAction);
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    if (_hasEnterTrigger)
    {
      return;
    }

    if (MustBeOnLadderToEnter
      && (GameManager.Instance.Player.PlayerState & PlayerState.ClimbingLadder) == 0)
    {
      return;
    }

    UpdatePlayerSpawnLocation();

    var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();

    OnEnter(boxCollider);
  }

  private void OnEnter(Collider2D collider)
  {
    if (_skipEnter)
    {
      _skipEnter = false;

      SetCameraMovementSettings(collider);

      _cameraController.MoveCameraToTargetPosition(GameManager.Instance.Player.transform.position);

      return;
    }

    var currentAnimatorStateInfo = GameManager.Instance.Player.Animator.GetCurrentAnimatorStateInfo(0);

    _animationShortNameHash = currentAnimatorStateInfo.shortNameHash;

    if (FullScreenScrollSettings.StartScrollFreezeTime > 0f)
    {
      GameManager.Instance.Player.PushControlHandler(
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          FullScreenScrollSettings.StartScrollFreezeTime,
          _animationShortNameHash));

      var delay = TimeSpan.FromSeconds(FullScreenScrollSettings.StartScrollFreezeTime);

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
      EnableDefaultVerticalLockPosition = EnableDefaultVerticalLockPosition,
      DefaultVerticalLockPosition = DefaultVerticalLockPosition,
      EnableTopVerticalLock = false,
      EnableBottomVerticalLock = false,
      TopVerticalLockPosition = transform.position.y + Size.y * .5f,
      BottomVerticalLockPosition = transform.position.y - Size.y * .5f
    };

    verticalLockSettings.TopBoundary =
      verticalLockSettings.TopVerticalLockPosition
      - _cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.BottomBoundary =
      verticalLockSettings.BottomVerticalLockPosition
      + _cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.TranslatedVerticalLockPosition =
      verticalLockSettings.DefaultVerticalLockPosition;

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