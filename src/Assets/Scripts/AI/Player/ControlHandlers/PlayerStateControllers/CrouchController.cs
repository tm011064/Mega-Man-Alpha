public class CrouchController : PlayerStateController
{
  private const float CROUCH_STANDUP_COLLISION_FUDGE_FACTOR = .0001f;

  public CrouchController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override void UpdateState(XYAxisState axisState)
  {
    if (!PlayerController.IsGrounded()
      || !PlayerController.CrouchSettings.EnableCrouching)
    {
      return;
    }

    if ((PlayerController.PlayerState & PlayerState.Crouching) != 0)
    {
      if (axisState.YAxis >= 0f
        && PlayerController.CharacterPhysicsManager.CanMoveVertically(
          PlayerController.EnvironmentBoxCollider.size.y
          - CROUCH_STANDUP_COLLISION_FUDGE_FACTOR, false))
      {
        PlayerController.PlayerState &= ~PlayerState.Crouching;

        return;
      }
    }
    else
    {
      if (axisState.YAxis < 0f)
      {
        PlayerController.PlayerState |= PlayerState.Crouching;

        Logger.Info(
          "Crouch executed, box collider size set to: " + PlayerController.CharacterPhysicsManager.BoxCollider.size
          + ", offset: " + PlayerController.CharacterPhysicsManager.BoxCollider.offset);
      }
    }
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.Crouching) == 0)
    {
      return PlayerStateUpdateResult.Unhandled;
    }

    if (axisState.IsInHorizontalSensitivityDeadZone())
    {
      return PlayerStateUpdateResult.CreateHandled("PlayerCrouchIdle");
    }

    return PlayerStateUpdateResult.CreateHandled("PlayerCrouchRun");
  }
}
