public class PlaySpecificAnimationPlayerStateController : PlayerStateController
{
  private readonly int _animationStateName;

  public PlaySpecificAnimationPlayerStateController(PlayerController playerController, int animationStateName)
    : base(playerController)
  {
    _animationStateName = animationStateName;
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if (!PlayerController.State.IsLocked())
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    return PlayerStateUpdateResult.CreateHandled(_animationStateName);
  }
}
