using UnityEngine;

public class FullScreenScroller : MonoBehaviour
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

  public float HorizontalOffsetDeltaMovementFactor = 40f;

  public VerticalCameraFollowMode VerticalCameraFollowMode;

  private GameObject _parent;

  private CameraController _cameraController;

  private VerticalLockSettings _verticalLockSettings;

  private HorizontalLockSettings _horizontalLockSettings;

  private bool _skipEnter;

  void Awake()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();

    var boxCollider = GetComponent<BoxCollider2D>();

    if (boxCollider == null)
    {
      throw new MissingComponentException("BoxCollider2D expected");
    }

    _horizontalLockSettings = CreateHorizontalLockSettings(boxCollider.bounds);

    _verticalLockSettings = CreateVerticalLockSettings(boxCollider.bounds);

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
          FullScreenScrollSettings.EndScrollFreezeTime));
    }

    GameManager.Instance.Player.PushControlHandler(
      new FreezePlayerControlHandler(
        GameManager.Instance.Player,
        FullScreenScrollSettings.TransitionTime,
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
    if (_skipEnter)
    {
      _skipEnter = false;

      SetCameraMovementSettings();

      return;
    }

    if (FullScreenScrollSettings.StartScrollFreezeTime > 0f)
    {
      GameManager.Instance.Player.PushControlHandler(
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          FullScreenScrollSettings.StartScrollFreezeTime));

      Invoke("StartScroll", FullScreenScrollSettings.StartScrollFreezeTime);
    }
    else
    {
      StartScroll();
    }
  }

  private Vector3 GetTransformPoint()
  {
    return RelativePositioningParentObject == null
      ? transform.parent.gameObject.transform.TransformPoint(Vector3.zero)
      : RelativePositioningParentObject.transform.TransformPoint(Vector3.zero);
  }

  private VerticalLockSettings CreateVerticalLockSettings(Bounds bounds)
  {
    var transformPoint = GetTransformPoint();

    var verticalLockSettings = new VerticalLockSettings
    {
      Enabled = true,
      EnableDefaultVerticalLockPosition = EnableDefaultVerticalLockPosition,
      DefaultVerticalLockPosition = DefaultVerticalLockPosition,
      EnableTopVerticalLock = true,
      EnableBottomVerticalLock = true,
      TopVerticalLockPosition = bounds.center.y + bounds.extents.y,
      BottomVerticalLockPosition = bounds.center.y - bounds.extents.y
    };

    verticalLockSettings.TopBoundary =
      transformPoint.y
      + verticalLockSettings.TopVerticalLockPosition
      - _cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.BottomBoundary =
      transformPoint.y
      + verticalLockSettings.BottomVerticalLockPosition
      + _cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.TranslatedVerticalLockPosition =
      transformPoint.y + verticalLockSettings.DefaultVerticalLockPosition;

    return verticalLockSettings;
  }

  private HorizontalLockSettings CreateHorizontalLockSettings(Bounds bounds)
  {
    var transformPoint = GetTransformPoint();

    var horizontalLockSettings = new HorizontalLockSettings
    {
      Enabled = true,
      EnableLeftHorizontalLock = true,
      EnableRightHorizontalLock = true,
      LeftHorizontalLockPosition = bounds.center.x - bounds.extents.x,
      RightHorizontalLockPosition = bounds.center.x + bounds.extents.x
    };

    horizontalLockSettings.LeftBoundary =
      transformPoint.x
      + horizontalLockSettings.LeftHorizontalLockPosition
      + _cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    horizontalLockSettings.RightBoundary =
      transformPoint.x
      + horizontalLockSettings.RightHorizontalLockPosition
      - _cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    return horizontalLockSettings;
  }
}