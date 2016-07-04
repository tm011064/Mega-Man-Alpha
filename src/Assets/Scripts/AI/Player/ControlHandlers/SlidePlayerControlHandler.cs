using UnityEngine;

public class SlidePlayerControlHandler : PlayerControlHandler
{
  private float _startTime;

  private float _distancePerSecond;

  public SlidePlayerControlHandler(PlayerController playerController)
    : base(playerController, new PlayerStateController[] { new SlideController(playerController) })
  {
    SetDebugDraw(Color.gray, true);
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.Sliding;

    _startTime = Time.time;

    _distancePerSecond = (1f / PlayerController.SlideSettings.Duration)
      * PlayerController.SlideSettings.Distance;

    if (!PlayerController.IsFacingRight())
    {
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
      PlayerController.StandIdleEnvironmentBoxColliderSize.y - PlayerController.BoxCollider.size.y;

    return CharacterPhysicsManager.CanMoveVertically(currentHeightToStandUprightHeightDelta);
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (_startTime + PlayerController.SlideSettings.Duration < Time.time
      && PlayerHasEnoughVerticalSpaceToGetUp())
    {
      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var deltaMovement = PlayerController.CharacterPhysicsManager.Velocity;

    deltaMovement.y = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(deltaMovement, PlayerController.AdjustedGravity, true),
      PlayerController.JumpSettings.MaxDownwardSpeed)
      * Time.deltaTime;

    deltaMovement.x = Time.deltaTime * _distancePerSecond;

    PlayerController.CharacterPhysicsManager.Move(deltaMovement);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
