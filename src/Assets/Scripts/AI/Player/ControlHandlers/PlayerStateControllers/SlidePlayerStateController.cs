public class SlidePlayerStateController : PlayerStateController
{
  public SlidePlayerStateController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.Sliding) == 0)
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    return PlayerStateUpdateResult.CreateHandled("Slide");
  }
}
