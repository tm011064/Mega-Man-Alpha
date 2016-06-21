using UnityEngine;

public class PowerUpFloaterControlHandler : PlayerControlHandler
{
  private PowerUpSettings _powerUpSettings;

  private FloatStatus _floatStatus;

  private float _originalInAirDamping;

  private bool _isFloating;

  public PowerUpFloaterControlHandler(PlayerController playerController, PowerUpSettings powerUpSettings)
    : base(playerController)
  {
    _powerUpSettings = powerUpSettings;
    _originalInAirDamping = PlayerController.JumpSettings.InAirDamping;

#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(.5f, 1f, 1f, 1f);
#endif
  }
  public override void Dispose()
  {
#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
#endif

    PlayerController.AdjustedGravity = PlayerController.JumpSettings.Gravity;
    PlayerController.JumpSettings.InAirDamping = _originalInAirDamping;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    CheckOneWayPlatformFallThrough();

    var velocity = PlayerController.CharacterPhysicsManager.Velocity;
    velocity.y = GetJumpVerticalVelocity(velocity);
    velocity.x = GetDefaultHorizontalVelocity(velocity);

    if (velocity.y == 0)
    {
      _floatStatus = FloatStatus.CanFloat;
    }
    else
    {
      _floatStatus |= FloatStatus.IsInAir;

      if (velocity.y < 0)
      { // player is moving down, so check floating logic
        if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsUp) != 0)
        {
          // player released jump button on way down, this means he can't float any more for this in air session
          _floatStatus &= ~FloatStatus.CanFloat;
        }
        if (((_floatStatus & FloatStatus.CanFloat) != 0)
          && (GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsDown) != 0)
        {
          // player is on his way down,can float and pressed the jump button, so he starts floating
          velocity.y *= _powerUpSettings.FloaterSettings.StartFloatingDuringFallVelocityMultiplier;
          _floatStatus |= FloatStatus.IsFloating;
        }
        if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsPressed) != 0)
        {
          // player is on his way down and has the jump button pressed, so set the floating field
          _floatStatus |= FloatStatus.IsFloating;
        }
      }
      else
      {
        _floatStatus |= FloatStatus.CanFloat;

        _floatStatus &= ~FloatStatus.IsFloating;
      }
    }

    var isFloating = velocity.y < 0
      && (_floatStatus == (FloatStatus.IsInAir | FloatStatus.CanFloat | FloatStatus.IsFloating));

    if (isFloating)
    {
      if (_isFloating)
      {
        PlayerController.AdjustedGravity = Mathf.Lerp(
          PlayerController.AdjustedGravity,
          PlayerController.JumpSettings.Gravity,
          Time.deltaTime * _powerUpSettings.FloaterSettings.FloaterGravityDecreaseInterpolationFactor);
      }
      else
      {
        PlayerController.AdjustedGravity = _powerUpSettings.FloaterSettings.FloaterGravity;
        PlayerController.JumpSettings.InAirDamping = _powerUpSettings.FloaterSettings.FloaterInAirDampingOverride;
      }
    }
    else
    {
      PlayerController.AdjustedGravity = PlayerController.JumpSettings.Gravity;
    }

    _isFloating = isFloating;

    velocity.y = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, true),
      PlayerController.JumpSettings.MaxDownwardSpeed);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }

  enum FloatStatus
  {
    IsInAir = 1,

    CanFloat = 2,

    IsFloating = 4
  }
}
