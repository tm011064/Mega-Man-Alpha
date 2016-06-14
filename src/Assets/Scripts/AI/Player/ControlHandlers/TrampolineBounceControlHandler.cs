using UnityEngine;

public class TrampolineBounceControlHandler : PlayerControlHandler
{
  public bool HasJumped { get { return _hasJumped; } }

  private float _onTrampolineSkidDamping;

  private bool _canJump = false;

  private bool _hasJumped = false;

  public TrampolineBounceControlHandler(PlayerController playerController, float duration, float fixedJumpHeight, float onTrampolineSkidDamping, bool canJump)
    : base(playerController, duration: duration)
  {
    FixedJumpHeight = fixedJumpHeight;

    _canJump = canJump;

    _onTrampolineSkidDamping = onTrampolineSkidDamping;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    if (!_hasJumped)
    {
      velocity.y = GetJumpVerticalVelocity(velocity, _canJump, out _hasJumped, ButtonPressState.IsPressed);
    }
    else
    {
      velocity.y = GetJumpVerticalVelocity(velocity, false);
    }

    velocity.y = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, false),
      PlayerController.JumpSettings.MaxDownwardSpeed);

    // whilst on trampoline, player must not move horizontally
    if (!_hasJumped)
    {
      velocity.x = Mathf.Lerp(velocity.x, 0f, _onTrampolineSkidDamping * Time.deltaTime);
    }
    else
    {
      velocity.x = GetDefaultHorizontalVelocity(velocity);
    }

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return (!_hasJumped) || velocity.y >= 0f
      ? ControlHandlerAfterUpdateStatus.KeepAlive
      : ControlHandlerAfterUpdateStatus.CanBeDisposed;
  }
}
