
public class GroundedController : PlayerStateController
{
  private readonly CrouchController _crouchController;

  public GroundedController(PlayerController playerController)
    : base(playerController)
  {
    if (playerController.CrouchSettings.EnableCrouching)
    {
      _crouchController = new CrouchController(PlayerController);
    }
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if (!PlayerController.IsGrounded())
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    if (PlayerController.CrouchSettings.EnableCrouching)
    {
      var result = _crouchController.UpdatePlayerState(axisState);

      if (result.IsHandled)
      {
        return result;
      }
    }

    if (axisState.IsInHorizontalSensitivityDeadZone())
    {
      return PlayerStateUpdateResult.CreateHandled("Idle");
    }

    return PlayerStateUpdateResult.CreateHandled("Run Start", linkedAnimationNames: new string[] { "Run" });
  }
}
