
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
      PlayerController.Animator.speed = 1;

      return PlayerStateUpdateResult.Unhandled;
    }

    PlayerController.Animator.speed =
      (PlayerController.PlayerState & PlayerState.ClimbingLadderTop) == 0
      && axisState.IsInVerticalSensitivityDeadZone()
        ? 0
        : 1; // TODO (Roman): maybe move this to result as well

    return (PlayerController.PlayerState & PlayerState.ClimbingLadderTop) != 0
      ? PlayerStateUpdateResult.CreateHandled("Climb Laddertop")
      : PlayerStateUpdateResult.CreateHandled("Climb");
  }
}
