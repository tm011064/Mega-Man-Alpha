using System.Collections.Generic;
using UnityEngine;

public class PlayerControlHandler : BaseControlHandler
{
  private const string TRACE_TAG = "PlayerControlHandler";

  private const float VERTICAL_COLLISION_FUDGE_FACTOR = .0001f;

  protected readonly PlayerController PlayerController;

  protected readonly PlayerMetricsSettings PlayerMetricSettings;

  protected float? FixedJumpHeight = null;

  protected bool HasPerformedGroundJumpThisFrame = false;

  protected bool HadDashPressedWhileJumpOff = false;

  protected AxisState HorizontalAxisOverride;

  protected AxisState VerticalAxisOverride;

  protected float JumpHeightMultiplier = 1f;

  private readonly PlayerStateController[] _animationStateControllers;

  public PlayerControlHandler(
    PlayerController playerController,
    PlayerStateController[] animationStateControllers = null,
    float duration = -1f)
    : base(playerController.CharacterPhysicsManager, duration)
  {
    PlayerController = playerController;
    PlayerMetricSettings = GameManager.Instance.GameSettings.PlayerMetricSettings;

    _animationStateControllers = animationStateControllers == null
      ? BuildPlayerStateControllers(playerController)
      : animationStateControllers;
  }

  private PlayerStateController[] BuildPlayerStateControllers(PlayerController playerController)
  {
    var controllers = new List<PlayerStateController>();

    if (playerController.SpinMeleeSettings.EnableSpinMelee)
    {
      controllers.Add(new SpinMeleeAttackController(playerController));
    }

    if (playerController.IsTakingDamageSettings.EnableTakingDamage)
    {
      controllers.Add(new IsTakingDamageController(playerController));
    }

    if (playerController.WallJumpSettings.EnableWallJumps)
    {
      controllers.Add(new WallSlideController(playerController));
    }

    controllers.Add(new GroundedController(playerController));
    controllers.Add(new AirborneController(playerController));

    return controllers.ToArray();
  }

  protected override void OnAfterUpdate()
  {
    Logger.Trace(TRACE_TAG, "OnAfterUpdate -> Velocity: " + CharacterPhysicsManager.Velocity);

    XYAxisState axisState;

    axisState.XAxis = HorizontalAxisOverride == null
      ? GameManager.InputStateManager.GetAxisState("Horizontal").Value
      : HorizontalAxisOverride.Value;

    axisState.YAxis = VerticalAxisOverride == null
      ? GameManager.InputStateManager.GetAxisState("Vertical").Value
      : VerticalAxisOverride.Value;

    axisState.SensibilityThreshold = .1f; // TODO (Roman): hardcoded

    for (var i = 0; i < _animationStateControllers.Length; i++)
    {
      if (_animationStateControllers[i].UpdateStateAndPlayAnimation(axisState) == AnimationPlayResult.Played)
      {
        return;
      }
    }
  }

  protected float GetGravityAdjustedVerticalVelocity(Vector3 velocity, float gravity, bool canBreakUpMovement)
  {
    // apply gravity before moving
    if (canBreakUpMovement && velocity.y > 0f
      && (GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsUp) != 0)
    {
      return (velocity.y + gravity * Time.deltaTime) * PlayerMetricSettings.JumpReleaseUpVelocityMultiplier;
    }
    else
    {
      return velocity.y + gravity * Time.deltaTime;
    }
  }

  protected float CalculateJumpHeight(Vector2 velocity)
  {
    if (FixedJumpHeight.HasValue)
    {
      return Mathf.Sqrt(2f * -PlayerController.JumpSettings.Gravity * FixedJumpHeight.Value);
    }
    else
    {
      var absVelocity = Mathf.Abs(velocity.x);

      float jumpHeight;

      if (absVelocity >= PlayerController.JumpSettings.RunJumpHeightSpeedTrigger)
      {
        jumpHeight = PlayerController.JumpSettings.RunJumpHeight;
      }
      else if (absVelocity >= PlayerController.JumpSettings.WalkJumpHeightSpeedTrigger)
      {
        jumpHeight = PlayerController.JumpSettings.WalkJumpHeight;
      }
      else
      {
        jumpHeight = PlayerController.JumpSettings.StandJumpHeight;
      }

      return Mathf.Sqrt(
        2f
        * JumpHeightMultiplier
        * -PlayerController.JumpSettings.Gravity
        * jumpHeight);
    }
  }

  protected float GetJumpVerticalVelocity(
    Vector3 velocity,
    bool canJump,
    out bool hasJumped,
    ButtonPressState allowedJumpButtonPressState = ButtonPressState.IsDown)
  {
    var value = velocity.y;

    hasJumped = false;

    HasPerformedGroundJumpThisFrame = false;

    if (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      HadDashPressedWhileJumpOff = false; // we set this to false here as the value is only used when player jumps off, not when he is grounded

      value = 0f;
    }

    if (canJump
      && (GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & allowedJumpButtonPressState) != 0)
    {
      if (CanJump())
      {
        value = CalculateJumpHeight(velocity);

        hasJumped = true;

        HasPerformedGroundJumpThisFrame = true;

        HadDashPressedWhileJumpOff = (GameManager.InputStateManager.GetButtonState("Dash").ButtonPressState & ButtonPressState.IsPressed) != 0;

        PlayerController.OnJumpedThisFrame();
      }
    }

    return value;
  }

  protected float GetJumpVerticalVelocity(Vector3 velocity, bool canJump)
  {
    bool hasJumped;

    return GetJumpVerticalVelocity(velocity, canJump, out hasJumped);
  }

  protected float GetJumpVerticalVelocity(Vector3 velocity)
  {
    bool hasJumped;

    return GetJumpVerticalVelocity(velocity, true, out hasJumped);
  }

  protected float GetNormalizedHorizontalSpeed(AxisState hAxis)
  {
    float normalizedHorizontalSpeed;

    if (hAxis.Value > 0f && hAxis.Value >= hAxis.LastValue)
    {
      normalizedHorizontalSpeed = 1;
    }
    else if (hAxis.Value < 0f && hAxis.Value <= hAxis.LastValue)
    {
      normalizedHorizontalSpeed = -1;
    }
    else
    {
      normalizedHorizontalSpeed = 0;
    }

    return normalizedHorizontalSpeed;
  }

  protected float GetHorizontalVelocityWithDamping(Vector3 velocity, float hAxis, float normalizedHorizontalSpeed)
  {
    var speed = PlayerController.RunSettings.WalkSpeed;
    if ((GameManager.InputStateManager.GetButtonState("Dash").ButtonPressState & ButtonPressState.IsPressed) != 0)
    {
      if ( // allow dash speed if
          PlayerController.RunSettings.EnableRunning // running is enabled
          && (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below // either the player is grounded
              || velocity.x > PlayerController.RunSettings.WalkSpeed   // or the current horizontal velocity is higher than the walkspeed, meaning that the player jumped while running
              || velocity.x < -PlayerController.RunSettings.WalkSpeed
              || HadDashPressedWhileJumpOff))
      {
        speed = PlayerController.RunSettings.RunSpeed;
      }
    }

    float smoothedMovementFactor;

    if (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      if (normalizedHorizontalSpeed == 0f)
      {
        smoothedMovementFactor = PlayerController.RunSettings.DecelerationGroundDamping;
      }
      else if (Mathf.Sign(normalizedHorizontalSpeed) == Mathf.Sign(velocity.x))
      {
        // accelerating...
        smoothedMovementFactor = PlayerController.RunSettings.AccelerationGroundDamping;
      }
      else
      {
        smoothedMovementFactor = PlayerController.RunSettings.DecelerationGroundDamping;
      }
    }
    else
    {
      smoothedMovementFactor = PlayerController.JumpSettings.InAirDamping;
    }

    var groundedAdjustmentFactor = PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below
      ? Mathf.Abs(hAxis)
      : 1f;

    var newVelocity = normalizedHorizontalSpeed * speed * groundedAdjustmentFactor;

    if (PlayerController.JumpSettings.EnableBackflipOnDirectionChange
      && HasPerformedGroundJumpThisFrame
      && Mathf.Sign(newVelocity) != Mathf.Sign(velocity.x))
    {
      // Note: this only works if the jump velocity calculation is done before the horizontal calculation!
      return normalizedHorizontalSpeed * PlayerController.JumpSettings.BackflipOnDirectionChangeSpeed;
    }

    return Mathf.Lerp(velocity.x, newVelocity, Time.deltaTime * smoothedMovementFactor);
  }

  protected float GetDefaultHorizontalVelocity(Vector3 velocity)
  {
    var horizontalAxis = HorizontalAxisOverride == null
      ? GameManager.InputStateManager.GetAxisState("Horizontal")
      : HorizontalAxisOverride;

    var normalizedHorizontalSpeed = GetNormalizedHorizontalSpeed(horizontalAxis);

    return GetHorizontalVelocityWithDamping(velocity, horizontalAxis.Value, normalizedHorizontalSpeed);
  }

  protected virtual bool CanJump()
  {
    var verticalRayDistance = (PlayerController.PlayerState & PlayerState.Crouching) != 0
      ? PlayerController.BoxColliderSizeDefault.y - PlayerController.CrouchSettings.BoxColliderSizeCrouched.y
      : VERTICAL_COLLISION_FUDGE_FACTOR;

    if (!CharacterPhysicsManager.CanMoveVertically(
      verticalRayDistance,
      (PlayerController.PlayerState & PlayerState.Crouching) == 0))
    {
      // if we crouch we don't allow edge slide up to simplify things
      return false;
    }

    return (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below
        || Time.time - PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.LastTimeGrounded < PlayerController.JumpSettings.AllowJumpAfterGroundLostThreashold);
  }

  protected void CheckOneWayPlatformFallThrough()
  {
    if (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below
      && (GameManager.InputStateManager.GetButtonState("Fall").ButtonPressState & ButtonPressState.IsPressed) != 0
      && PlayerController.CurrentPlatform != null
      && PlayerController.CurrentPlatform.layer == LayerMask.NameToLayer("OneWayPlatform"))
    {
      var oneWayPlatform = PlayerController.CurrentPlatform.GetComponent<OneWayPlatform>();

      Logger.Assert(
        oneWayPlatform != null,
        "OneWayPlatform " + PlayerController.CurrentPlatform.name + " has no 'OneWayPlatform' script attached. This script is needed in order to allow the player to fall through.");

      if (oneWayPlatform != null)
      {
        oneWayPlatform.TriggerFall();
      }
    }
  }

  public override void DrawGizmos()
  {
    if (DoDrawDebugBoundingBox)
    {
      GizmoUtility.DrawBoundingBox(
        PlayerController.transform.position + PlayerController.BoxCollider.offset.ToVector3(),
        PlayerController.BoxCollider.bounds.extents, DebugBoundingBoxColor);
    }
  }
}
