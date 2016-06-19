using UnityEngine;

public class CrouchController : PlayerStateController
{
  private const float CROUCH_STANDUP_COLLISION_FUDGE_FACTOR = .0001f;

  public CrouchController(PlayerController playerController)
    : base(playerController)
  {
  }

  private void GetUp()
  {
    PlayerController.CharacterPhysicsManager.BoxCollider.offset = PlayerController.BoxColliderOffsetDefault;
    PlayerController.CharacterPhysicsManager.BoxCollider.size = PlayerController.BoxColliderSizeDefault;

    PlayerController.CharacterPhysicsManager.RecalculateDistanceBetweenRays();

    Logger.Info("Crouch ended, box collider size set to: " + PlayerController.CharacterPhysicsManager.BoxCollider.size + ", offset: " + PlayerController.CharacterPhysicsManager.BoxCollider.offset);
  }

  public override void UpdateState(XYAxisState axisState)
  {
    if (!PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below
      || !PlayerController.CrouchSettings.EnableCrouching)
    {
      return;
    }

    if ((PlayerController.PlayerState & PlayerState.Crouching) != 0)
    {
      if (axisState.YAxis >= 0f
        && PlayerController.CharacterPhysicsManager.CanMoveVertically(
          PlayerController.BoxColliderSizeDefault.y
          - PlayerController.CrouchSettings.BoxColliderSizeCrouched.y
          - CROUCH_STANDUP_COLLISION_FUDGE_FACTOR, false))
      {
        GetUp();

        PlayerController.PlayerState &= ~PlayerState.Crouching;

        return;
      }
    }
    else
    {
      if (axisState.YAxis < 0f)
      {
        // TODO (Roman): do this via mecanim
        PlayerController.CharacterPhysicsManager.BoxCollider.offset = PlayerController.CrouchSettings.BoxColliderOffsetCrouched;
        PlayerController.CharacterPhysicsManager.BoxCollider.size = PlayerController.CrouchSettings.BoxColliderSizeCrouched;

        PlayerController.CharacterPhysicsManager.RecalculateDistanceBetweenRays();

        PlayerController.PlayerState |= PlayerState.Crouching;

        Logger.Info(
          "Crouch executed, box collider size set to: " + PlayerController.CharacterPhysicsManager.BoxCollider.size
          + ", offset: " + PlayerController.CharacterPhysicsManager.BoxCollider.offset);
      }
    }
  }

  public override AnimationPlayResult PlayAnimation(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.Crouching) == 0)
    {
      return AnimationPlayResult.NotPlayed;
    }

    if (axisState.IsInHorizontalSensitivityDeadZone())
    {
      PlayerController.Animator.Play(Animator.StringToHash("PlayerCrouchIdle"));
    }
    else
    {
      PlayerController.Animator.Play(Animator.StringToHash("PlayerCrouchRun"));
    }

    return AnimationPlayResult.Played;
  }
}
