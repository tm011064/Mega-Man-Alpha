using UnityEngine;

public class ClimbController : PlayerStateController
{
  public ClimbController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override AnimationPlayResult PlayAnimation(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.ClimbingLadder) == 0)
    {
      PlayerController.Animator.speed = 1;

      return AnimationPlayResult.NotPlayed;
    }

    var animatorState = PlayerController.Animator.GetCurrentAnimatorStateInfo(0);

    if (!animatorState.IsName("Climb"))
    {
      PlayerController.Animator.Play(Animator.StringToHash("Climb"));
    }

    PlayerController.Animator.speed = axisState.IsInVerticalSensitivityDeadZone()
      ? 0
      : 1;

    return AnimationPlayResult.Played;
  }
}
