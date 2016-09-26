
public class ClimbController : PlayerStateController
{
  public ClimbController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if (!PlayerController.State.IsClimbingLadder()
      && !PlayerController.State.IsClimbingLadderTop())
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    var animationSpeed = !PlayerController.State.IsClimbingLadderTop()
      && axisState.IsInVerticalSensitivityDeadZone()
        ? 0f
        : 1f;

    return PlayerController.State.IsClimbingLadderTop()
      ? PlayerStateUpdateResult.CreateHandled("Climb Laddertop", animationSpeed: animationSpeed)
      : PlayerStateUpdateResult.CreateHandled("Climb", animationSpeed: animationSpeed);
  }
}
