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

  public bool MustBeOnLadderToEnter;

  public float HorizontalOffsetDeltaMovementFactor = 40f;

  [Tooltip("The boundary padding valeus reduce the scroll dimensions for the camera. This is necessary for room transitions where a room's box collider extends into another room which should not be visible once entry has happened")]
  public Padding BoundaryPadding;

  public VerticalCameraFollowMode VerticalCameraFollowMode;

  private GameObject _parent;

  private CameraController _cameraController;

  private VerticalLockSettings _verticalLockSettings;

  private HorizontalLockSettings _horizontalLockSettings;

  private bool _skipEnter;

  private int _animationShortNameHash;

  private Checkpoint _checkpoint;

  void Awake()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();

    _checkpoint = GetComponentInChildren<Checkpoint>();

    var boxCollider = GetComponent<BoxCollider2D>();

    if (boxCollider == null)
    {
      throw new MissingComponentException("BoxCollider2D expected");
    }

    _horizontalLockSettings = CreateHorizontalLockSettings(boxCollider.bounds);

    _verticalLockSettings = CreateVerticalLockSettings(boxCollider.bounds);

    OnSceneReset();
  }

  public void OnSceneReset()
  {
    var boxCollider = GetComponent<BoxCollider2D>();

    _skipEnter = boxCollider.bounds.Contains(GameManager.Instance.Player.transform.position);
  }

  private void SetCameraMovementSettings()
  {
    var cameraMovementSettings = new CameraMovementSettings(
      _verticalLockSettings,
      _horizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      Offset,
      VerticalCameraFollowMode,
      HorizontalOffsetDeltaMovementFactor);

    _cameraController.SetCameraMovementSettings(cameraMovementSettings);
  }

  private void StartScroll()
  {
    SetCameraMovementSettings();

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
    if (MustBeOnLadderToEnter
      && (GameManager.Instance.Player.PlayerState & PlayerState.ClimbingLadder) == 0)
    {
      return;
    }

    UpdatePlayerSpawnLocation();

    if (_skipEnter)
    {
      _skipEnter = false;

      SetCameraMovementSettings();

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

      Invoke("StartScroll", FullScreenScrollSettings.StartScrollFreezeTime);
    }
    else
    {
      StartScroll();
    }
  }

  private void UpdatePlayerSpawnLocation()
  {
    if (_checkpoint != null)
    {
      GameManager.Instance.Player.SpawnLocation = _checkpoint.transform.position;
    }
  }

  private VerticalLockSettings CreateVerticalLockSettings(Bounds bounds)
  {
    var verticalLockSettings = new VerticalLockSettings
    {
      Enabled = true,
      EnableDefaultVerticalLockPosition = EnableDefaultVerticalLockPosition,
      DefaultVerticalLockPosition = DefaultVerticalLockPosition,
      EnableTopVerticalLock = false,
      EnableBottomVerticalLock = false,
      TopVerticalLockPosition = bounds.center.y + bounds.extents.y - BoundaryPadding.Top,
      BottomVerticalLockPosition = bounds.center.y - bounds.extents.y + BoundaryPadding.Bottom
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

  private HorizontalLockSettings CreateHorizontalLockSettings(Bounds bounds)
  {
    var horizontalLockSettings = new HorizontalLockSettings
    {
      Enabled = true,
      EnableLeftHorizontalLock = true,
      EnableRightHorizontalLock = true,
      LeftHorizontalLockPosition = bounds.center.x - bounds.extents.x + BoundaryPadding.Left,
      RightHorizontalLockPosition = bounds.center.x + bounds.extents.x - BoundaryPadding.Right
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