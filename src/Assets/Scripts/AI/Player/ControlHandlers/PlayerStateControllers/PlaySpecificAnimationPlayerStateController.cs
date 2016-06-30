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
    if ((PlayerController.PlayerState & PlayerState.Locked) == 0)
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    return PlayerStateUpdateResult.CreateHandled(_animationStateName);
  }
}
