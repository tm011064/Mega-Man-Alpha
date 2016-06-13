using UnityEngine;

public class SpinMeleeAttackController : PlayerStateController
{
  public SpinMeleeAttackController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override void UpdateState(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.PerformingSpinMeleeAttack) == 0)
    {
      return;
    }

    // we need to check whether the animation has finished. If so, we set the flag which will 
    // allow the player to do another attack.
    var animatorStateInfo = PlayerController.Animator.GetCurrentAnimatorStateInfo(0);

    if (animatorStateInfo.IsName("PlayerSpinMeleeAttack"))
    {
      if (animatorStateInfo.normalizedTime > 1f)
      {
        PlayerController.PlayerState &= ~PlayerState.PerformingSpinMeleeAttack;
      }
    }
  }

  public override AnimationPlayResult PlayAnimation(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.PerformingSpinMeleeAttack) == 0)
    {
      return AnimationPlayResult.NotPlayed;
    }

    PlayerController.Animator.Play(Animator.StringToHash("PlayerSpinMeleeAttack"));

    return AnimationPlayResult.Played;
  }
}

