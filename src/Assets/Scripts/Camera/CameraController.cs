using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Vector3 CameraOffset;

  public bool UseFixedUpdate = false;

  public Vector2 TargetScreenSize = new Vector2(1920, 1080);

  public bool UseFixedAspectRatio = false;

  public Vector2 FixedAspectRatio = new Vector2(4, 3);

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

  private bool _isAboveJumpHeightLocked;

  private readonly Queue<TranslateTransformAction> _scrollActions = new Queue<TranslateTransformAction>(16);

  private TranslateTransformAction _activeTranslateTransformAction;

  public void EnqueueScrollAction(TranslateTransformAction scrollAction)
  {
    _scrollActions.Enqueue(scrollAction);
  }

  public void SetPosition(Vector3 position)
  {
    Transform.position = position;

    _lastTargetPosition = position;

    _targetedTransformPositionX = _lastTargetPosition.x;

    _horizontalSmoothDampVelocity = _verticalSmoothDampVelocity = 0f;
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

  public Vector3 CalculateTargetPosition()
  {
    UpdateParameters updateParameters;

    return CalculateTargetPosition(out updateParameters);
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
    if (UseFixedAspectRatio)
    {
      AssignFixedAspectRatio(FixedAspectRatio.x / FixedAspectRatio.y);
    }

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

  void UpdateCameraPosition()
  {
    if (IsCameraControlledByScrollAction())
    {
      return;
    }

    UpdateParameters updateParameters;

    var targetPositon = CalculateTargetPosition(out updateParameters);

    _targetedTransformPositionX = updateParameters.XPos;

    Transform.position = new Vector3(
      Mathf.SmoothDamp(
        Transform.position.x,
        targetPositon.x,
        ref _horizontalSmoothDampVelocity,
        _cameraMovementSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime),
      Mathf.SmoothDamp(
        Transform.position.y,
        targetPositon.y,
        ref _verticalSmoothDampVelocity,
        updateParameters.VerticalSmoothDampTime),
      targetPositon.z);

    _lastTargetPosition = Target.transform.position;
  }

  private void AssignFixedAspectRatio(float targetAspectRatio)
  {
    var windowaspect = (float)Screen.width / (float)Screen.height;

    var scaleheight = windowaspect / targetAspectRatio;

    var camera = GetComponent<Camera>();

    if (scaleheight < 1.0f) // add letterbox
    {
      var rect = camera.rect;

      rect.width = 1.0f;
      rect.height = scaleheight;
      rect.x = 0;
      rect.y = (1.0f - scaleheight) / 2.0f;

      camera.rect = rect;
    }
    else // add pillarbox
    {
      var scalewidth = 1.0f / scaleheight;

      var rect = camera.rect;

      rect.width = scalewidth;
      rect.height = 1.0f;
      rect.x = (1.0f - scalewidth) / 2.0f;
      rect.y = 0;

      camera.rect = rect;
    }
  }

  private Vector3 CalculateTargetPosition(out UpdateParameters updateParameters)
  {
    updateParameters = new UpdateParameters();

    CalculateVerticalCameraPosition(ref updateParameters);

    CalculateHorizontalCameraPosition(ref updateParameters);

    return new Vector3(
      updateParameters.XPos,
      updateParameters.YPos - CameraOffset.y,
      Target.position.z - CameraOffset.z);
  }

  private bool IsCameraControlledByScrollAction()
  {
    if (_activeTranslateTransformAction == null
      && _scrollActions.Any())
    {
      _activeTranslateTransformAction = _scrollActions.Dequeue();

      _activeTranslateTransformAction.Start(transform.position);
    }

    if (_activeTranslateTransformAction == null)
    {
      return false;
    }

    transform.position = _activeTranslateTransformAction.GetPosition();

    if (_activeTranslateTransformAction.ActionStatus == TranslateTransformActionStatus.Completed)
    {
      _activeTranslateTransformAction = _scrollActions.Any()
        ? _scrollActions.Dequeue()
        : null;
    }

    return true;
  }

  private void AdjustFollowWhenGroundedParameters(ref UpdateParameters updateParameters)
  {
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
      updateParameters.YPos = _cameraMovementSettings.VerticalLockSettings.TopBoundary + CameraOffset.y;

      updateParameters.DoSmoothDamp = true;
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
        updateParameters.YPos = Target.position.y - _playerController.JumpSettings.RunJumpHeight;

        _isAboveJumpHeightLocked = true; // make sure for second if condition
      }
      else
      {
        updateParameters.IsFallingDown = (_characterPhysicsManager.Velocity.y < 0f
           && (Target.position.y < Transform.position.y + CameraOffset.y));

        if (_characterPhysicsManager.LastMoveCalculationResult.CollisionState.Below
          || updateParameters.IsFallingDown)
        {
          if (_cameraMovementSettings.VerticalLockSettings.Enabled)
          {
            updateParameters.YPos = _cameraMovementSettings.VerticalLockSettings.EnableDefaultVerticalLockPosition
              ? _cameraMovementSettings.VerticalLockSettings.TranslatedVerticalLockPosition
              : Target.position.y;

            if (_cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
              && Target.position.y > _cameraMovementSettings.VerticalLockSettings.TopBoundary)
            {
              updateParameters.YPos = _cameraMovementSettings.VerticalLockSettings.TopBoundary + CameraOffset.y;

              // we might have been shot up, so use smooth damp override
              updateParameters.DoSmoothDamp = true;
            }
            else if (_cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
              && Target.position.y < _cameraMovementSettings.VerticalLockSettings.BottomBoundary)
            {
              updateParameters.YPos = _cameraMovementSettings.VerticalLockSettings.BottomBoundary + CameraOffset.y;

              // we might have been falling down, so use smooth damp override
              updateParameters.DoSmoothDamp = true;
            }
          }
          else
          {
            updateParameters.YPos = Target.position.y;
          }
        }
        else
        {
          // character is in air, so the camera stays same
          updateParameters.YPos = Transform.position.y + CameraOffset.y; // we need to add offset bceause we will deduct it later on again
        }
      }
    }
  }

  private void AdjustFollowAlwaysParameters(ref UpdateParameters updateParameters)
  {
    _isAboveJumpHeightLocked = false; // this is not used at this mode

    if (_cameraMovementSettings.VerticalLockSettings.Enabled)
    {
      updateParameters.YPos = _cameraMovementSettings.VerticalLockSettings.EnableDefaultVerticalLockPosition
        ? _cameraMovementSettings.VerticalLockSettings.TranslatedVerticalLockPosition
        : Target.position.y;

      if (_cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
        && Target.position.y > _cameraMovementSettings.VerticalLockSettings.TopBoundary + CameraOffset.y)
      {
        updateParameters.YPos = _cameraMovementSettings.VerticalLockSettings.TopBoundary + CameraOffset.y;

        // we might have been shot up, so use smooth damp override
        updateParameters.DoSmoothDamp = true;
      }
      else if (_cameraMovementSettings.VerticalLockSettings.EnableBottomVerticalLock
        && Target.position.y < _cameraMovementSettings.VerticalLockSettings.BottomBoundary + CameraOffset.y)
      {
        updateParameters.YPos = _cameraMovementSettings.VerticalLockSettings.BottomBoundary + CameraOffset.y;

        // we might have been falling down, so use smooth damp override
        updateParameters.DoSmoothDamp = true;
      }
    }
    else
    {
      updateParameters.YPos = Target.position.y;
    }
  }

  private bool IsCameraOnTrolley(ref UpdateParameters updateParameters)
  {
    if (_cameraTrolleys == null)
    {
      return false;
    }

    for (var i = 0; i < _cameraTrolleys.Length; i++)
    {
      if (!_cameraTrolleys[i].IsPlayerWithinBoundingBox)
      {
        continue;
      }

      float? posY = _cameraTrolleys[i].GetPositionY(Target.position.x);

      if (posY.HasValue)
      {
        updateParameters.YPos = posY.Value;

        return true;
      }

      break;
    }

    return false;
  }

  private bool NeedsHorizontalOffsetAdjustment(ref UpdateParameters updateParameters)
  {
    if (_cameraMovementSettings.HorizontalLockSettings.Enabled)
    {
      if (_cameraMovementSettings.HorizontalLockSettings.EnableRightHorizontalLock
        && Target.position.x > _cameraMovementSettings.HorizontalLockSettings.RightBoundary - CameraOffset.x)
      {
        updateParameters.XPos = _cameraMovementSettings.HorizontalLockSettings.RightBoundary;

        return false;
      }
      else if (_cameraMovementSettings.HorizontalLockSettings.EnableLeftHorizontalLock
        && Target.position.x < _cameraMovementSettings.HorizontalLockSettings.LeftBoundary + CameraOffset.x)
      {
        updateParameters.XPos = _cameraMovementSettings.HorizontalLockSettings.LeftBoundary;

        return false;
      }
    }

    return CameraOffset.x != 0f;
  }

  private void AdjustHorizontalOffset(ref UpdateParameters updateParameters)
  {
    var horizontalTargetDistance = Target.transform.position.x - _lastTargetPosition.x;

    updateParameters.XPos = _targetedTransformPositionX;

    if ((horizontalTargetDistance >= -.001f && horizontalTargetDistance <= .001f)
      || CameraOffset.x >= 0f)
    {
      return;
    }

    updateParameters.XPos =
      _targetedTransformPositionX
      + horizontalTargetDistance * _cameraMovementSettings.HorizontalOffsetDeltaMovementFactor;

    if (horizontalTargetDistance > 0f) // going right
    {
      if (updateParameters.XPos + CameraOffset.x > Target.position.x)
      {
        updateParameters.XPos = Target.position.x - CameraOffset.x;
      }

      if (_cameraMovementSettings.HorizontalLockSettings.EnableRightHorizontalLock
        && updateParameters.XPos > _cameraMovementSettings.HorizontalLockSettings.RightBoundary)
      {
        updateParameters.XPos = _cameraMovementSettings.HorizontalLockSettings.RightBoundary;
      }
    }
    else // going left
    {
      if (updateParameters.XPos - CameraOffset.x < Target.position.x)
      {
        updateParameters.XPos = Target.position.x + CameraOffset.x;
      }

      if (_cameraMovementSettings.HorizontalLockSettings.EnableLeftHorizontalLock
        && updateParameters.XPos < _cameraMovementSettings.HorizontalLockSettings.LeftBoundary)
      {
        updateParameters.XPos = _cameraMovementSettings.HorizontalLockSettings.LeftBoundary;
      }
    }
  }

  private void CalculateVerticalCameraPosition(ref UpdateParameters updateParameters)
  {
    if (IsCameraOnTrolley(ref updateParameters))
    {
      updateParameters.VerticalSmoothDampTime = _cameraMovementSettings.SmoothDampMoveSettings.VerticalSmoothDampTime;

      return;
    }

    switch (_cameraMovementSettings.VerticalCameraFollowMode)
    {
      case VerticalCameraFollowMode.FollowWhenGrounded:
        AdjustFollowWhenGroundedParameters(ref updateParameters);
        break;

      case VerticalCameraFollowMode.FollowAlways:
      default:
        AdjustFollowAlwaysParameters(ref updateParameters);
        break;
    }

    updateParameters.VerticalSmoothDampTime = updateParameters.DoSmoothDamp // override
      ? _cameraMovementSettings.SmoothDampMoveSettings.VerticalSmoothDampTime
      : updateParameters.IsFallingDown
        ? _cameraMovementSettings.SmoothDampMoveSettings.VerticalRapidDescentSmoothDampTime
        : _isAboveJumpHeightLocked
          ? _cameraMovementSettings.SmoothDampMoveSettings.VerticalAboveRapidAcsentSmoothDampTime
          : _cameraMovementSettings.SmoothDampMoveSettings.VerticalSmoothDampTime;
  }

  private void CalculateHorizontalCameraPosition(ref UpdateParameters updateParameters)
  {
    updateParameters.XPos = Target.position.x;

    if (NeedsHorizontalOffsetAdjustment(ref updateParameters))
    {
      AdjustHorizontalOffset(ref updateParameters);
    }
  }

  private struct UpdateParameters
  {
    public float YPos;

    public float XPos;

    public bool DoSmoothDamp;

    public bool IsFallingDown;

    public float VerticalSmoothDampTime;
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
