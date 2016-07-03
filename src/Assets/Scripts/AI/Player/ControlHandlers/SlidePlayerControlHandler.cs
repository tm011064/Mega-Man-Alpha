using UnityEngine;

public class SlidePlayerControlHandler : PlayerControlHandler
{
  private float _startTime;

  private float _directionMultiplier;

  private float _startPosX;

  public SlidePlayerControlHandler(PlayerController playerController)
    : base(playerController, new PlayerStateController[] { new SlideController(playerController) })
  {
    SetDebugDraw(Color.gray, true);
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.Sliding;

    _startTime = Time.time;

    _directionMultiplier = PlayerController.IsFacingRight()
      ? 1f
      : -1f;

    _startPosX = PlayerController.transform.position.x;

    return true;
  }

  public override void Dispose()
  {
    PlayerController.CharacterPhysicsManager.Velocity.x = 0f;

    PlayerController.PlayerState &= ~PlayerState.Sliding;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (_startTime + PlayerController.SlideSettings.Duration < Time.time)
    {
      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var percentage = GameManager.Easing.GetValue(
      PlayerController.SlideSettings.EasingType,
      Time.time - _startTime,
      PlayerController.SlideSettings.Duration);

    var deltaMovement = PlayerController.CharacterPhysicsManager.Velocity;

    deltaMovement.y = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(deltaMovement, PlayerController.AdjustedGravity, true),
      PlayerController.JumpSettings.MaxDownwardSpeed)
      * Time.deltaTime;

    var targetPosX = _startPosX
      + PlayerController.SlideSettings.Distance * percentage * _directionMultiplier;

    deltaMovement.x = targetPosX - PlayerController.transform.position.x;

    PlayerController.CharacterPhysicsManager.Move(deltaMovement);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
