using UnityEngine;

public abstract class WeaponControlHandler
{
  protected readonly PlayerController PlayerController;

  protected readonly GameManager GameManager;

  protected WeaponControlHandler(PlayerController playerController)
  {
    PlayerController = playerController;

    GameManager = GameManager.Instance;
  }

  public abstract bool IsAttacking();

  public abstract PlayerStateUpdateResult Update(XYAxisState axisState);

  protected Vector2 GetDirectionVector(XYAxisState axisState)
  {
    return (
                axisState.IsInHorizontalSensitivityDeadZone()
             && PlayerController.IsFacingRight()
           )
           || axisState.XAxis > 0f
      ? Vector2.right
      : -Vector2.right;
  }
}