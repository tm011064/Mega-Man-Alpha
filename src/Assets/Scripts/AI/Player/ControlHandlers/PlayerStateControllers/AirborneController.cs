using UnityEngine;

public class AirborneController : PlayerStateController
{
  public AirborneController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override void UpdateState(XYAxisState axisState)
  {
    if (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      return;
    }

    AdjustSpriteScale(axisState);
  }

  public override AnimationPlayResult PlayAnimation(XYAxisState axisState)
  {
    if (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      return AnimationPlayResult.NotPlayed;
    }

    if (PlayerController.CharacterPhysicsManager.Velocity.y >= 0f)
    {
      PlayerController.Animator.Play(Animator.StringToHash("Jump"));
    }
    else
    {
      PlayerController.Animator.Play(Animator.StringToHash("Fall"));
    }

    return AnimationPlayResult.Played;
  }
}
