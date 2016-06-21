using UnityEngine;

public class DefaultPlayerControlHandler : PlayerControlHandler
{
  public DefaultPlayerControlHandler(PlayerController playerController)
    : base(playerController)
  {
  }

  public DefaultPlayerControlHandler(
    PlayerController playerController, 
    PlayerStateController[] animationStateControllers, 
    float duration)
    : base(playerController, animationStateControllers, duration)
  {
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    CheckOneWayPlatformFallThrough();

    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    velocity.y = GetJumpVerticalVelocity(velocity);
    velocity.y = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, true),
      PlayerController.JumpSettings.MaxDownwardSpeed);

    velocity.x = GetDefaultHorizontalVelocity(velocity);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    Logger.Trace("PlayerMetricsDebug", "Position: " + PlayerController.transform.position + ", Velocity: " + velocity);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}

