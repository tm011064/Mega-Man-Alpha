using UnityEngine;

public class DamageTakenPlayerControlHandler : DefaultPlayerControlHandler
{
  private float _suspendPhysicsEndTime;

  public DamageTakenPlayerControlHandler()
    : this(
        GameManager.Instance.Player,
        GameManager.Instance.GameSettings.PlayerDamageControlHandlerSettings.Duration,
        GameManager.Instance.GameSettings.PlayerDamageControlHandlerSettings.SuspendPhysicsTime)
  {
  }

  public DamageTakenPlayerControlHandler(PlayerController playerController, float duration, float suspendPhysicsTime)
    : base(playerController, duration)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.red;

    playerController.PlayerState |= PlayerState.Invincible;
    playerController.PlayerState |= PlayerState.TakingDamage;

    _suspendPhysicsEndTime = Time.time + suspendPhysicsTime;
  }

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.Invincible;
    PlayerController.PlayerState &= ~PlayerState.TakingDamage;
  }

  protected override bool DoUpdate()
  {
    if (Time.time <= _suspendPhysicsEndTime)
    {
      return true;
    }
    else
    {
      PlayerController.PlayerState &= ~PlayerState.TakingDamage;
    }

    return base.DoUpdate();
  }
}
