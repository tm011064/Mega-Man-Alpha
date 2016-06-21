using UnityEngine;

public class TopBounceableControlHandler : DefaultPlayerControlHandler
{
  private bool _hasPerformedDefaultBounce = false;

  private float _bounceJumpMultiplier;

  public TopBounceableControlHandler(PlayerController playerController, float duration, float bounceJumpMultiplier)
    : base(playerController, null, duration)
  {
    _bounceJumpMultiplier = bounceJumpMultiplier;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    if (!_hasPerformedDefaultBounce)
    {
      if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsPressed) != 0)
      {
        velocity.y = CalculateJumpHeight(velocity);

        Logger.Info("Top bounce jump executed. Jump button was pressed. New velocity y: " + velocity.y);

        PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

        return ControlHandlerAfterUpdateStatus.CanBeDisposed; // exit, we are done
      }
      else
      {
        velocity.y = Mathf.Sqrt(2f * PlayerController.JumpSettings.WalkJumpHeight * -PlayerController.JumpSettings.Gravity) * _bounceJumpMultiplier;

        _hasPerformedDefaultBounce = true;

        PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

        Logger.Info("Top bounce jump executed. Jump button was not pressed. BounceJumpMultiplier: " + _bounceJumpMultiplier + ", new velocity y: " + velocity.y);

        return ControlHandlerAfterUpdateStatus.KeepAlive; // keep waiting, maybe user presses jump before time is up
      }
    }
    if ((GameManager.InputStateManager.GetButtonState("Jump").ButtonPressState & ButtonPressState.IsPressed) != 0)
    {
      velocity.y = CalculateJumpHeight(velocity);

      Logger.Info("Top bounce jump executed. Jump button was pressed. New velocity y: " + velocity.y);

      PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return base.DoUpdate();
  }
}