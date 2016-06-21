using UnityEngine;

public class DamageTakenPlayerControlHandler : DefaultPlayerControlHandler
{
  private float _suspendPhysicsEndTime;

  private readonly float _suspendPhysicsTime;

  public DamageTakenPlayerControlHandler()
    : this(
        GameManager.Instance.Player,
        GameManager.Instance.GameSettings.PlayerDamageControlHandlerSettings.Duration,
        GameManager.Instance.GameSettings.PlayerDamageControlHandlerSettings.SuspendPhysicsTime)
  {
  }

  public DamageTakenPlayerControlHandler(PlayerController playerController, float duration, float suspendPhysicsTime)
    : base(playerController, null, duration)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.red;

    _suspendPhysicsTime = suspendPhysicsTime;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.Invincible;
    PlayerController.PlayerState |= PlayerState.TakingDamage;

    _suspendPhysicsEndTime = Time.time + _suspendPhysicsTime;

    return true;
  }

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.Invincible;
    PlayerController.PlayerState &= ~PlayerState.TakingDamage;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (Time.time <= _suspendPhysicsEndTime)
    {
      return ControlHandlerAfterUpdateStatus.KeepAlive;
    }
    else
    {
      PlayerController.PlayerState &= ~PlayerState.TakingDamage;
    }

    return base.DoUpdate();
  }
}
