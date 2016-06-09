using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Vector3 CameraOffset;

  public bool UseFixedUpdate = false;

  public Vector2 TargetScreenSize = new Vector2(1920, 1080);

  [HideInInspector]
  public Transform Target;

  [HideInInspector]
  public Transform Transform;

  private CharacterPhysicsManager _characterPhysicsManager;

  private float _horizontalSmoothDampVelocity;

  private float _verticalSmoothDampVelocity;

  private CameraMovementSettings _cameraMovementSettings;

  private UpdateTimer _zoomTimer;

  private PlayerController _playerController;

  private CameraTrolley[] _cameraTrolleys;

  private Vector3 _lastTargetPosition;

  private float _targetedTransformPositionX;

  private bool _isAboveJumpHeightLocked = false;

  public void SetPosition(Vector3 position)
  {
    Transform.position = position;

    _lastTargetPosition = position;

    _targetedTransformPositionX = _lastTargetPosition.x;

    _horizontalSmoothDampVelocity = _verticalSmoothDampVelocity = 0f;
  }

  void Reset()
  {
    Logger.Info("Resetting camera movement settings.");

    var cameraMovementSettings = new CameraMovementSettings(
      new VerticalLockSettings(),
      new HorizontalLockSettings(),
      new ZoomSettings(),
      new SmoothDampMoveSettings(),
      Vector2.zero,
      VerticalCameraFollowMode.FollowAlways,
      0f);

    SetCameraMovementSettings(cameraMovementSettings);
  }

  void Start()
  {
    if (_cameraTrolleys == null)
    {
      _cameraTrolleys = FindObjectsOfType<CameraTrolley>();

      Debug.Log("Found " + _cameraTrolleys.Length + " camera trolleys.");
    }

    Transform = gameObject.transform;

    _playerController = GameManager.Instance.Player;

    // we set the target of the camera to our player through code
    Target = _playerController.transform;

    _lastTargetPosition = Target.transform.position;

    _targetedTransformPositionX = _lastTargetPosition.x;

    _characterPhysicsManager = Target.GetComponent<CharacterPhysicsManager>();

    Logger.Info("Window size: " + Screen.width + " x " + Screen.height);

    Reset();
  }

  void Update()
  {
    if (_zoomTimer != null)
    {
      _zoomTimer.Update();

      if (_zoomTimer.HasEnded)
      {
        _zoomTimer = null;
      }
    }
  }

  void LateUpdate()
  {
    if (!UseFixedUpdate)
    {
      UpdateCameraPosition();
    }
  }

  void FixedUpdate()
  {
    if (UseFixedUpdate)
    {
      UpdateCameraPosition();
    }
  }

  public bool IsPointVisible(Vector2 point)
  {
    var defaultOrthographicSize = (TargetScreenSize.y * .5f);

    var zoomPercentage = Camera.main.orthographicSize / defaultOrthographicSize;

    var screenSize = new Vector2(
      (float)TargetScreenSize.x * zoomPercentage,
      (float)TargetScreenSize.y * zoomPercentage);

    var rect = new Rect(
      Camera.main.transform.position.x - screenSize.x * .5f,
      Camera.main.transform.position.y - screenSize.y * .5f,
      screenSize.x,
      screenSize.y);

    return rect.Contains(point);
  }

  public void SetCameraMovementSettings(CameraMovementSettings cameraMovementSettings)
  {
    _cameraMovementSettings = cameraMovementSettings;

    CameraOffset = new Vector3(
      _cameraMovementSettings.Offset.x,
      _cameraMovementSettings.Offset.y,
      CameraOffset.z);

    var targetOrthographicSize = (TargetScreenSize.y * .5f) / _cameraMovementSettings.ZoomSettings.ZoomPercentage;

    if (!Mathf.Approximately(Camera.main.orthographicSize, targetOrthographicSize))
    {
      Logger.Info("Start zoom to target size: " + targetOrthographicSize + ", current size: " + Camera.main.orthographicSize);

      if (_cameraMovementSettings.ZoomSettings.ZoomTime == 0f)
      {
        Camera.main.orthographicSize = targetOrthographicSize;
      }
      else
      {
        _zoomTimer = new ZoomTimer(_cameraMovementSettings.ZoomSettings.ZoomTime, Camera.main.orthographicSize, targetOrthographicSize, _cameraMovementSettings.ZoomSettings.ZoomEasingType);

        _zoomTimer.Start();
      }
    }

    Logger.Info("Camera movement set to: " + _cameraMovementSettings.ToString());

    Logger.Info("Camera size; current: " + Camera.main.orthographicSize + ", target: " + targetOrthographicSize);
  }

  void UpdateCameraPosition()
  {
    var yPos = 0f;
    var xPos = 0f;

    var isOnCameraTrolley = false;

    if (_cameraTrolleys != null)
    {
      for (var i = 0; i < _cameraTrolleys.Length; i++)
      {
        if (_cameraTrolleys[i].IsPlayerWithinBoundingBox)
        {
          float? posY = _cameraTrolleys[i].GetPositionY(Target.position.x);

          if (posY.HasValue)
          {
            yPos = posY.Value;

            isOnCameraTrolley = true;
          }

          break;
        }
      }
    }

    var verticalSmoothDampTime = _cameraMovementSettings.SmoothDampMoveSettings.VerticalSmoothDampTime;

    if (!isOnCameraTrolley)
    {
      var isFallingDown = false;

      var doSmoothDamp = false;

      switch (_cameraMovementSettings.VerticalCameraFollowMode)
      {
        case VerticalCameraFollowMode.FollowWhenGrounded:

          if (_isAboveJumpHeightLocked && _characterPhysicsManager.Velocity.y < 0f)
          {
            // We set this value to true in order to make the camera follow the character upwards when catapulted above the maximum jump height. The
            // character can not exceed the maximum jump heihgt without help (trampoline, powerup...).
            _isAboveJumpHeightLocked = false; // if we reached the peak we unlock
          }

          if (_isAboveJumpHeightLocked
            && (_cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
             && Target.position.y > _cameraMovementSettings.VerticalLockSettings.TopBoundary))
          {
            // we were locked but character has exceeded the top boundary. In that case we set the y pos and smooth damp
            yPos = _cameraMovementSettings.VerticalLockSettings.TopBoundary + CameraOffset.y;

            doSmoothDamp = true;
          }
          else
          {
            // we want to adjust the y position on upward movement if:
            if (_isAboveJumpHeightLocked // either we are locked in above jump height lock
                || (
                      (
                        !_cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock // OR (we either have no top boundary or we are beneath the top boundary in which case we can go up)
                        || Target.position.y <= _cameraMovementSettings.VerticalLockSettings.TopBoundary)
                      &&
                      (
                        Target.position.y > Transform.position.y + CameraOffset.y + _playerController.JumpSettings.RunJumpHeight // (the character has exceeded the jump height which means he has been artifically catapulted upwards)
                        && _characterPhysicsManager.Velocity.y > 0f // AND we go up  
                      )
                   )
              )
            {
              yPos = Target.position.y - _playerController.JumpSettings.RunJumpHeight;

              _isAboveJumpHeightLocked = true; // make sure for second if condition
            }
            else
            {
              isFallingDown = (_characterPhysicsManager.Velocity.y < 0f
                 && (Target.position.y < Transform.position.y + CameraOffset.y));

              if (_characterPhysicsManager.LastMoveCalculationResult.CollisionState.Below
                || isFallingDown)
              {
                if (_cameraMovementSettings.VerticalLockSettings.Enabled)
                {
                  yPos = _cameraMovementSettings.VerticalLockSettings.EnableDefaultVerticalLockPosition
                    ? _cameraMovementSettings.VerticalLockSettings.TranslatedVerticalLockPosition
                    : Target.position.y;

                  if (_cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
                    && Target.position.y > _cameraMovementSettings.VerticalLockSettings.TopBoundary)
                  {
                    yPos = _cameraMovementSettings.VerticalLockSettings.TopBoundary + CameraOffset.y;

                    // we might have been shot up, so use smooth damp override
                    doSmoothDamp = true;
                  }
                  else if (_cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
                    && Target.position.y < _cameraMovementSettings.VerticalLockSettings.BottomBoundary)
                  {
                    yPos = _cameraMovementSettings.VerticalLockSettings.BottomBoundary + CameraOffset.y;

                    // we might have been falling down, so use smooth damp override
                    doSmoothDamp = true;
                  }
                }
                else
                {
                  yPos = Target.position.y;
                }
              }
              else
              {
                // character is in air, so the camera stays same
                yPos = Transform.position.y + CameraOffset.y; // we need to add offset bceause we will deduct it later on again
              }
            }
          }

          break;

        case VerticalCameraFollowMode.FollowAlways:
        default:

          _isAboveJumpHeightLocked = false; // this is not used at this mode

          if (_cameraMovementSettings.VerticalLockSettings.Enabled)
          {
            yPos = _cameraMovementSettings.VerticalLockSettings.EnableDefaultVerticalLockPosition
              ? _cameraMovementSettings.VerticalLockSettings.TranslatedVerticalLockPosition
              : Target.position.y;

            if (_cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
              && Target.position.y > _cameraMovementSettings.VerticalLockSettings.TopBoundary + CameraOffset.y)
            {
              yPos = _cameraMovementSettings.VerticalLockSettings.TopBoundary + CameraOffset.y;

              // we might have been shot up, so use smooth damp override
              doSmoothDamp = true;
            }
            else if (_cameraMovementSettings.VerticalLockSettings.EnableBottomVerticalLock
              && Target.position.y < _cameraMovementSettings.VerticalLockSettings.BottomBoundary + CameraOffset.y)
            {
              yPos = _cameraMovementSettings.VerticalLockSettings.BottomBoundary + CameraOffset.y;

              // we might have been falling down, so use smooth damp override
              doSmoothDamp = true;
            }
          }
          else
          {
            yPos = Target.position.y;
          }

          break;
      }

      verticalSmoothDampTime = doSmoothDamp // override
        ? _cameraMovementSettings.SmoothDampMoveSettings.VerticalSmoothDampTime
        : isFallingDown
          ? _cameraMovementSettings.SmoothDampMoveSettings.VerticalRapidDescentSmoothDampTime
          : _isAboveJumpHeightLocked
            ? _cameraMovementSettings.SmoothDampMoveSettings.VerticalAboveRapidAcsentSmoothDampTime
            : _cameraMovementSettings.SmoothDampMoveSettings.VerticalSmoothDampTime;
    }

    var xTargetDelta = Target.transform.position.x - _lastTargetPosition.x;

    xPos = Target.position.x;

    var doAdjustHorizontalOffset = CameraOffset.x != 0f;

    if (_cameraMovementSettings.HorizontalLockSettings.Enabled)
    {
      if (_cameraMovementSettings.HorizontalLockSettings.EnableRightHorizontalLock
        && Target.position.x > _cameraMovementSettings.HorizontalLockSettings.RightBoundary - CameraOffset.x)
      {
        xPos = _cameraMovementSettings.HorizontalLockSettings.RightBoundary;

        doAdjustHorizontalOffset = false;
      }
      else if (_cameraMovementSettings.HorizontalLockSettings.EnableLeftHorizontalLock
        && Target.position.x < _cameraMovementSettings.HorizontalLockSettings.LeftBoundary + CameraOffset.x)
      {
        xPos = _cameraMovementSettings.HorizontalLockSettings.LeftBoundary;

        doAdjustHorizontalOffset = false;
      }
    }

    if (doAdjustHorizontalOffset)
    {
      xPos = _targetedTransformPositionX;

      if ((xTargetDelta < -.001f
          || xTargetDelta > .001f))
      {
        if (CameraOffset.x < 0f)
        {
          xPos =
            _targetedTransformPositionX
            + xTargetDelta * _cameraMovementSettings.HorizontalOffsetDeltaMovementFactor;

          if (xTargetDelta > 0f) // going right
          {
            if (xPos + CameraOffset.x > Target.position.x)
            {
              xPos = Target.position.x - CameraOffset.x;
            }

            if (_cameraMovementSettings.HorizontalLockSettings.EnableRightHorizontalLock
              && xPos > _cameraMovementSettings.HorizontalLockSettings.RightBoundary)
            {
              xPos = _cameraMovementSettings.HorizontalLockSettings.RightBoundary;
            }
          }
          else // going left
          {
            if (xPos - CameraOffset.x < Target.position.x)
            {
              xPos = Target.position.x + CameraOffset.x;
            }

            if (_cameraMovementSettings.HorizontalLockSettings.EnableLeftHorizontalLock
              && xPos < _cameraMovementSettings.HorizontalLockSettings.LeftBoundary)
            {
              xPos = _cameraMovementSettings.HorizontalLockSettings.LeftBoundary;
            }
          }
        }
      }
    }

    _targetedTransformPositionX = xPos;

    var targetPositon = new Vector3(xPos, yPos - CameraOffset.y, Target.position.z - CameraOffset.z);

    Transform.position = new Vector3(
      Mathf.SmoothDamp(Transform.position.x, targetPositon.x, ref _horizontalSmoothDampVelocity, _cameraMovementSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime),
      Mathf.SmoothDamp(Transform.position.y, targetPositon.y, ref _verticalSmoothDampVelocity, verticalSmoothDampTime),
      targetPositon.z);

    _lastTargetPosition = Target.transform.position;
  }

  class ZoomTimer : UpdateTimer
  {
    private float _startSize;

    private float _targetSize;

    private EasingType _easingType;

    private Easing _easing;

    public ZoomTimer(float duration, float startSize, float targetSize, EasingType easingType)
      : base(duration)
    {
      _startSize = startSize;
      _targetSize = targetSize;
      _easingType = easingType;
      _easing = new Easing();
    }

    protected override void DoUpdate(float currentTime)
    {
      var value = _easing.GetValue(_easingType, currentTime, _duration);

      Camera.main.orthographicSize = _startSize + (_targetSize - _startSize) * value;
    }
  }
}
