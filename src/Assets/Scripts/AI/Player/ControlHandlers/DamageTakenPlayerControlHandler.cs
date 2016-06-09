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

    playerController.IsInvincible = true;
    playerController.IsTakingDamage = true;

    _suspendPhysicsEndTime = Time.time + suspendPhysicsTime;
  }

  public override void Dispose()
  {
    PlayerController.IsInvincible = false;
    PlayerController.IsTakingDamage = false;
  }

  protected override bool DoUpdate()
  {
    if (Time.time <= _suspendPhysicsEndTime)
    {
      return true;
    }
    else
    {
      PlayerController.IsTakingDamage = false;
    }

    return base.DoUpdate();
  }
}
