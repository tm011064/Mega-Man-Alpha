
public class ClimbController : PlayerStateController
{
  public ClimbController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.ClimbingLadder) == 0
      && (PlayerController.PlayerState & PlayerState.ClimbingLadderTop) == 0)
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    var animationSpeed = (PlayerController.PlayerState & PlayerState.ClimbingLadderTop) == 0
      && axisState.IsInVerticalSensitivityDeadZone()
        ? 0f
        : 1f;

    return (PlayerController.PlayerState & PlayerState.ClimbingLadderTop) != 0
      ? PlayerStateUpdateResult.CreateHandled("Climb Laddertop", animationSpeed: animationSpeed)
      : PlayerStateUpdateResult.CreateHandled("Climb", animationSpeed: animationSpeed);
  }
}
