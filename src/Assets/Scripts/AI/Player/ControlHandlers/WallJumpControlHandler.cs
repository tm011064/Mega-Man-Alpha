using UnityEngine;

public class WallJumpControlHandler : PlayerControlHandler
{
  private const string TRACE_TAG = "WallJumpControlHandler";

  private bool _hasJumpedFromWall;

  private float _wallJumpDirectionMultiplier;

  private WallJumpSettings _wallJumpSettings;

  private AxisState _axisOverride;

  public WallJumpControlHandler(PlayerController playerController)
    : base(playerController)
  {
    SetDebugDraw(Color.cyan, true);
  }

  public void Reset(float duration, Direction wallDirection, WallJumpSettings wallJumpSettings)
  {
    OverrideEndTime = duration < 0f
      ? null
      : (float?)(Time.time + duration);

    _hasJumpedFromWall = false;

    _wallJumpSettings = wallJumpSettings;

    _wallJumpDirectionMultiplier = wallDirection == Direction.Right
      ? -1f
      : 1f;

    _axisOverride = new AxisState(-_wallJumpDirectionMultiplier);
  }

  public override void Dispose()
  {
    PlayerController.AdjustedGravity = PlayerController.JumpSettings.Gravity;

    PlayerController.PlayerState &= ~PlayerState.AttachedToWall;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    var wallJumpEvaluationControlHandler = previousControlHandler as WallJumpEvaluationControlHandler;

    if (wallJumpEvaluationControlHandler == null)
    {
      Logger.Info("Wall Jump Control Handler can not be activated because previous control handler is null.");

      return false;
    }
    else if (wallJumpEvaluationControlHandler.HasDetached)
    {
      Logger.Info("Wall Jump Control Handler can not be activated because wall jump evaluation control handler has detached.");

      return false;
    }

    PlayerController.PlayerState |= PlayerState.AttachedToWall;

    return base.TryActivate(previousControlHandler);
  }

  public override string ToString()
  {
    return "WallJumpControlHandler; override end time: " + OverrideEndTime + "; has jumped from wall: " + _hasJumpedFromWall;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (PlayerController.IsGrounded())
    {
      Logger.Info("Popped wall jump because player is grounded.");

      return ControlHandlerAfterUpdateStatus.CanBeDisposed; // we only want this handler to be active while the player is in mid air
    }

    if (PlayerController.WallJumpSettings.MinDistanceFromFloor > 0f && PlayerController.CharacterPhysicsManager.IsFloorWithinDistance(PlayerController.WallJumpSettings.MinDistanceFromFloor))
    {
      Logger.Info("Popped wall jump because player is within min threshold distance (" + PlayerController.WallJumpSettings.MinDistanceFromFloor + " units) to floor.");

      return ControlHandlerAfterUpdateStatus.CanBeDisposed; // we only want this handler to be active while the player is in mid air
    }

    // TODO (Roman): when wall jumping and floating, we should not be able to climb up a wall using wall jumps and floats
    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    if (velocity.y < _wallJumpSettings.WallVelocityDownThreshold)
    {
      Logger.Info("Popped wall jump because downward velocity threshold was surpassed: " + velocity.y + " < " + _wallJumpSettings.WallVelocityDownThreshold);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed; // we can exit as wall jump is not allowed any more after the player accelerated downward beyond threshold
    }

    if (!_hasJumpedFromWall
      && (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.CharacterWallState & CharacterWallState.NotOnWall) != 0)
    {
      Logger.Info("Popped wall jump because character is not on wall any more.");

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (velocity.y <= 0f)
    {
      PlayerController.AdjustedGravity = _wallJumpSettings.WallStickGravity; // going down, use wall stick gravity     
    }
    else
    {
      PlayerController.AdjustedGravity = PlayerController.JumpSettings.Gravity; // still going up, use normal gravity
    }

    var isWallJump = false;

    if (!_hasJumpedFromWall
        && ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsDown) != 0))
    {
      // set flag for later calcs outside this scope
      isWallJump = true;

      // we want to override x axis buttons for a certain amount of time so the sprite can push off from the wall
      OverrideEndTime = Time.time + _wallJumpSettings.WallJumpPushOffAxisOverrideTime;

      // set jump height as usual
      velocity.y = Mathf.Sqrt(2f * PlayerController.JumpSettings.WalkJumpHeight * -PlayerController.JumpSettings.Gravity);

      // disable jump
      _hasJumpedFromWall = true;

      _axisOverride = new AxisState(_wallJumpDirectionMultiplier);

      Logger.Info("Wall Jump executed. Velocity y: " + velocity.y);
    }

    var normalizedHorizontalSpeed = GetNormalizedHorizontalSpeed(_axisOverride);

    if (isWallJump)
    {
      velocity.x = normalizedHorizontalSpeed * PlayerController.RunSettings.WalkSpeed;
    }
    else
    {
      velocity.x = GetHorizontalVelocityWithDamping(velocity, _axisOverride.Value, normalizedHorizontalSpeed);
    }

    // we need to check whether we have to adjust the vertical velocity. If levitation is on, we want to defy the law of physics
    velocity.y = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, true),
      PlayerController.WallJumpSettings.MaxWallDownwardSpeed);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}

