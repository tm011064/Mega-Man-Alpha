using UnityEngine;

public class TrampolineAutoBounceControlHandler : PlayerControlHandler
{
  private bool _isFirstUpdate = true;

  public TrampolineAutoBounceControlHandler(PlayerController playerController, float fixedJumpHeight)
    : base(playerController, duration: -1f)
  {
    FixedJumpHeight = fixedJumpHeight;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    if (_isFirstUpdate)
    {
      velocity.y = CalculateJumpHeight(velocity);

      HadDashPressedWhileJumpOff =
        (GameManager.InputStateManager.GetButtonState("Dash").ButtonPressState & ButtonPressState.IsPressed) != 0;

      _isFirstUpdate = false;
    }

    velocity.y = Mathf.Max(
        GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, false),
        PlayerController.JumpSettings.MaxDownwardSpeed);

    velocity.x = GetDefaultHorizontalVelocity(velocity);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return velocity.y >= 0f
      ? ControlHandlerAfterUpdateStatus.KeepAlive
      : ControlHandlerAfterUpdateStatus.CanBeDisposed;
  }
}