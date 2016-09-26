public class SlidePlayerStateController : PlayerStateController
{
  public SlidePlayerStateController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if (!PlayerController.IsSliding())
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    return PlayerStateUpdateResult.CreateHandled("Slide");
  }
}
