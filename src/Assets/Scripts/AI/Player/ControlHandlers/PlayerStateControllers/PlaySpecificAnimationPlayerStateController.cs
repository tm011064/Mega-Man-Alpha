public class PlaySpecificAnimationPlayerStateController : PlayerStateController
{
  private int _animationStateNameHash;

  public PlaySpecificAnimationPlayerStateController(PlayerController playerController, int animationStateNameHash)
    : base(playerController)
  {
    _animationStateNameHash = animationStateNameHash;
  }

  public override AnimationPlayResult PlayAnimation(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.Locked) == 0)
    {
      return AnimationPlayResult.NotPlayed;
    }

    PlayerController.Animator.Play(_animationStateNameHash);

    return AnimationPlayResult.Played;
  }
}
