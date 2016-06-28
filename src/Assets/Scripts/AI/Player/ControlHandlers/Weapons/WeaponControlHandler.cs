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

  public abstract void Update();

  protected Vector2 GetDirectionVector()
  {
    var axisState = GetAxisState();

    return (
                axisState.IsInHorizontalSensitivityDeadZone()
             && PlayerController.IsFacingRight()
           )
           || axisState.XAxis > 0f
      ? Vector2.right
      : -Vector2.right;
  }

  protected XYAxisState GetAxisState()
  {
    XYAxisState axisState;

    axisState.XAxis = GameManager.InputStateManager.GetHorizontalAxisState().Value;

    axisState.YAxis = GameManager.InputStateManager.GetVerticalAxisState().Value;

    axisState.SensitivityThreshold = PlayerController.InputSettings.AxisSensitivityThreshold;

    return axisState;
  }
}