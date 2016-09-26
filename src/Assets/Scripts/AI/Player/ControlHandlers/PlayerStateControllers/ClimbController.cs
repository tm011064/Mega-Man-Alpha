
public class ClimbController : PlayerStateController
{
  public ClimbController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if (!PlayerController.IsClimbingLadder()
      && !PlayerController.IsClimbingLadderTop())
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    var animationSpeed = !PlayerController.IsClimbingLadderTop()
      && axisState.IsInVerticalSensitivityDeadZone()
        ? 0f
        : 1f;

    return PlayerController.IsClimbingLadderTop()
      ? PlayerStateUpdateResult.CreateHandled("Climb Laddertop", animationSpeed: animationSpeed)
      : PlayerStateUpdateResult.CreateHandled("Climb", animationSpeed: animationSpeed);
  }
}
