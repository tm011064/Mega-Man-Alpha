using UnityEngine;

public class WallJumpEvaluationControlHandler : DefaultPlayerControlHandler
{
  private const string TRACE_TAG = "WallJumpEvaluationControlHandler";

  private Direction _wallDirection;

  private WallJumpSettings _wallJumpSettings;

  private bool _hasDetached = false;

  public WallJumpEvaluationControlHandler(PlayerController playerController)
    : base(playerController)
  {
    SetDebugDraw(Color.cyan, true);
  }

  public bool HasDetached { get { return _hasDetached; } }

  public void Reset(float duration, Direction wallDirection, WallJumpSettings wallJumpSettings)
  {
    OverrideEndTime = Time.time + duration;

    _wallDirection = wallDirection;

    _wallJumpSettings = wallJumpSettings;

    _hasDetached = false;
  }

  public override string ToString()
  {
    return "WallJumpEvaluationControlHandler; override end time: " + OverrideEndTime + "; has detached from wall: " + _hasDetached;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (PlayerController.IsGrounded())
    {
      _hasDetached = true;

      Logger.Info("Popped wall jump evaluation because player is grounded.");

      return ControlHandlerAfterUpdateStatus.CanBeDisposed; // we only want this handler to be active while the player is in mid air
    }

    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    if (velocity.y < _wallJumpSettings.WallVelocityDownThreshold)
    {
      _hasDetached = true;

      Logger.Info("Popped wall jump evaluation because downward velocity threshold was surpassed: " + velocity.y + " < " 
        + _wallJumpSettings.WallVelocityDownThreshold);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed; // we can exit as wall jump is not allowed any more after the player accelerated downward beyond threshold
    }

    if ((PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.CharacterWallState & CharacterWallState.NotOnWall) != 0)
    {
      _hasDetached = true;

      Logger.Info("Popped wall jump evaluation because character is not on wall any more.");

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var hAxis = GameManager.InputStateManager.GetHorizontalAxisState();

    if (_wallDirection == Direction.Right)
    {
      if (hAxis.Value <= 0f || hAxis.Value < hAxis.LastValue)
      {
        _hasDetached = true;

        Logger.Info("Popped wall jump evaluation because horizontal axis points to opposite direction. Current and Last axis value: (" + hAxis.Value + ", " + hAxis.LastValue + ")");

        return ControlHandlerAfterUpdateStatus.CanBeDisposed;
      }
    }
    else if (_wallDirection == Direction.Left)
    {
      if (hAxis.Value >= 0f || hAxis.Value > hAxis.LastValue)
      {
        _hasDetached = true;

        Logger.Info("Popped wall jump evaluation because horizontal axis points to opposite direction. Current and Last axis value: (" + hAxis.Value + ", " + hAxis.LastValue + ")");

        return ControlHandlerAfterUpdateStatus.CanBeDisposed;
      }
    }
    else
    {
      _hasDetached = true;

      Logger.Info("Popped wall jump evaluation because direction " + _wallDirection + " is not supported.");

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return base.DoUpdate();
  }
}

