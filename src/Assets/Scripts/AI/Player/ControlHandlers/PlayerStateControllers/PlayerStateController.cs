using UnityEngine;

public abstract class PlayerStateController
{
  protected readonly PlayerController PlayerController;

  protected PlayerStateController(PlayerController playerController)
  {
    PlayerController = playerController;
  }

  public abstract AnimationPlayResult PlayAnimation(XYAxisState axisState);

  public virtual void UpdateState(XYAxisState axisState)
  {
  }

  public AnimationPlayResult UpdateStateAndPlayAnimation(XYAxisState axisState)
  {
    UpdateState(axisState);

    return PlayAnimation(axisState);
  }

  protected void AdjustSpriteScale(XYAxisState axisState)
  {
    if ((axisState.XAxis > 0f && PlayerController.Sprite.transform.localScale.x < 1f)
      || (axisState.XAxis < 0f && PlayerController.Sprite.transform.localScale.x > -1f))
    {
      PlayerController.Sprite.transform.localScale = new Vector3(
        PlayerController.Sprite.transform.localScale.x * -1,
        PlayerController.Sprite.transform.localScale.y,
        PlayerController.Sprite.transform.localScale.z);
    }
  }

  protected void StartAnimationIfNotAlreadyStarted(string name, params string[] transitionAnimationNames)
  {
    var animationInfo = PlayerController.Animator.GetCurrentAnimatorStateInfo(0);

    if (animationInfo.IsName(name))
    {
      return;
    }

    foreach (var animationName in transitionAnimationNames)
    {
      if (animationInfo.IsName(animationName))
      {
        return;
      }
    }

    PlayerController.Animator.Play(Animator.StringToHash(name));
  }
}
