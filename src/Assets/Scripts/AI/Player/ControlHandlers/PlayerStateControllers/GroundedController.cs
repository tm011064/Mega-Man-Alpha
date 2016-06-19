using UnityEngine;

public class GroundedController : PlayerStateController
{
  private readonly CrouchController _crouchController;

  public GroundedController(PlayerController playerController)
    : base(playerController)
  {
    _crouchController = new CrouchController(PlayerController);
  }

  public override AnimationPlayResult PlayAnimation(XYAxisState axisState)
  {
    if (!PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      return AnimationPlayResult.NotPlayed;
    }

    if (_crouchController.UpdateStateAndPlayAnimation(axisState) == AnimationPlayResult.Played)
    {
      return AnimationPlayResult.Played;
    }

    if (axisState.IsInHorizontalSensitivityDeadZone())
    {
      PlayerController.Animator.Play(Animator.StringToHash("Idle"));
    }
    else
    {
      AdjustSpriteScale(axisState);

      PlayerController.Animator.Play(Animator.StringToHash("Run"));
    }

    return AnimationPlayResult.Played;
  }
}
