using UnityEngine;

public class SlidePlayerControlHandler : PlayerControlHandler
{
  private float _startTime;

  private float _distancePerSecond;

  public SlidePlayerControlHandler(PlayerController playerController)
    : base(playerController, new PlayerStateController[] { new SlidePlayerStateController(playerController) })
  {
    SetDebugDraw(Color.gray, true);
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.Sliding;

    _startTime = Time.time;

    _distancePerSecond = (1f / PlayerController.SlideSettings.Duration)
      * PlayerController.SlideSettings.Distance;

    HorizontalAxisOverride = new AxisState(1f);

    if (!PlayerController.IsFacingRight())
    {
      HorizontalAxisOverride.Value = -1f;

      _distancePerSecond *= -1f;
    }

    return true;
  }

  public override void Dispose()
  {
    PlayerController.CharacterPhysicsManager.Velocity.x = 0f;

    PlayerController.PlayerState &= ~PlayerState.Sliding;
  }

  private bool PlayerHasEnoughVerticalSpaceToGetUp()
  {
    var currentHeightToStandUprightHeightDelta =
      PlayerController.StandIdleEnvironmentBoxColliderSize.y - PlayerController.EnvironmentBoxCollider.size.y;

    return CharacterPhysicsManager.CanMoveVertically(currentHeightToStandUprightHeightDelta);
  }

  private void HandleDirectionChange()
  {
    HorizontalAxisOverride = null;

    var axisState = GetAxisState();

    if (!axisState.IsInHorizontalSensitivityDeadZone())
    {
      if (_distancePerSecond > 0f && axisState.XAxis < 0f
        || _distancePerSecond < 0f && axisState.XAxis > 0f)
      {
        _distancePerSecond = -_distancePerSecond;
      }
    }
  }

  private Vector2 CalculateDeltaMovement()
  {
    return new Vector2(
      Time.deltaTime * _distancePerSecond,
      Mathf.Max(
        GetGravityAdjustedVerticalVelocity(
          PlayerController.CharacterPhysicsManager.Velocity,
          PlayerController.AdjustedGravity,
          true),
        PlayerController.JumpSettings.MaxDownwardSpeed)
      * Time.deltaTime);
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (_startTime + PlayerController.SlideSettings.Duration < Time.time)
    {
      if (PlayerHasEnoughVerticalSpaceToGetUp())
      {
        return ControlHandlerAfterUpdateStatus.CanBeDisposed;
      }

      HandleDirectionChange();
    }

    if (!PlayerController.IsGrounded())
    {
      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var deltaMovement = CalculateDeltaMovement();

    PlayerController.CharacterPhysicsManager.Move(deltaMovement);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
