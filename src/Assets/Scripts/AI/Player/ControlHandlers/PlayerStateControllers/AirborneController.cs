public class AirborneController : PlayerStateController
{
  public AirborneController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if (PlayerController.IsGrounded())
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    return PlayerController.CharacterPhysicsManager.Velocity.y >= 0f
      ? PlayerStateUpdateResult.CreateHandled("Jump")
      : PlayerStateUpdateResult.CreateHandled("Fall");
  }
}
