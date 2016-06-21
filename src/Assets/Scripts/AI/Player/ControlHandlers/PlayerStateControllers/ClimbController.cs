using UnityEngine;

public class ClimbController : PlayerStateController
{
  public ClimbController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override AnimationPlayResult PlayAnimation(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.ClimbingLadder) == 0
      && (PlayerController.PlayerState & PlayerState.ClimbingLadderTop) == 0)
    {
      PlayerController.Animator.speed = 1;

      return AnimationPlayResult.NotPlayed;
    }

    var animatorState = PlayerController.Animator.GetCurrentAnimatorStateInfo(0);

    var animationName = (PlayerController.PlayerState & PlayerState.ClimbingLadderTop) != 0
      ? "Climb Laddertop"
      : "Climb";

    Debug.Log("PLAY: " + animationName);
    if (!animatorState.IsName(animationName))
    {
      PlayerController.Animator.Play(Animator.StringToHash(animationName));
    }

    PlayerController.Animator.speed = axisState.IsInVerticalSensitivityDeadZone()
      ? 0
      : 1;

    return AnimationPlayResult.Played;
  }
}
