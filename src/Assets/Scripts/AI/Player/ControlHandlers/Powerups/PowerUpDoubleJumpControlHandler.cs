using UnityEngine;

public class PowerUpDoubleJumpControlHandler : PlayerControlHandler
{
  private bool _canDoubleJump;

  private bool _hasUsedDoubleJump;

  public PowerUpDoubleJumpControlHandler(PlayerController playerController)
    : base(playerController)
  {
#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 0.75f, 1f, 1f);
#endif
  }

  public override void Dispose()
  {
#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
#endif
  }

  protected override bool DoUpdate()
  {
    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    if (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      _canDoubleJump = true;
    }

    velocity.y = GetJumpVerticalVelocity(velocity);

    if (PlayerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      velocity.y = 0f;
    }

    if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsDown) != 0)
    {
      if (CanJump())
      {
        velocity.y = CalculateJumpHeight(velocity);

        Logger.Info("Ground Jump executed. Velocity y: " + velocity.y);
      }
      else if (_canDoubleJump)
      {
        velocity.y = CalculateJumpHeight(velocity);

        Logger.Info("Double Jump executed. Velocity y: " + velocity.y);

        _canDoubleJump = false;
      }
    }

    velocity.y = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, true),
      PlayerController.JumpSettings.MaxDownwardSpeed);

    velocity.x = GetDefaultHorizontalVelocity(velocity);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return true;
  }
}
