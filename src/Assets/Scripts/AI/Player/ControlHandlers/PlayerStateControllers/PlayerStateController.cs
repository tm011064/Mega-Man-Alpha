using UnityEngine;

public abstract class PlayerStateController
{
  protected readonly PlayerController PlayerController;

  protected PlayerStateController(PlayerController playerController)
  {
    PlayerController = playerController;
  }

  public abstract PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState);

  public virtual void UpdateState(XYAxisState axisState)
  {
  }

  public PlayerStateUpdateResult UpdatePlayerState(XYAxisState axisState)
  {
    UpdateState(axisState);

    return GetPlayerStateUpdateResult(axisState);
  }
}
