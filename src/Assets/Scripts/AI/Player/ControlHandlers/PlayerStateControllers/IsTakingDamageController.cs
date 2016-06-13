using UnityEngine;

public class IsTakingDamageController : PlayerStateController
{
  public IsTakingDamageController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override AnimationPlayResult PlayAnimation(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.TakingDamage) == 0)
    {
      return AnimationPlayResult.NotPlayed;
    }

    PlayerController.Animator.Play(Animator.StringToHash("PlayerDamageTaken"));

    return AnimationPlayResult.Played;
  }
}
