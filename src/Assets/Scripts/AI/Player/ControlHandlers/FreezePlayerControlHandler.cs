using UnityEngine;

public class FreezePlayerControlHandler : DefaultPlayerControlHandler
{
  private float _suspendPhysicsEndTime;

  public FreezePlayerControlHandler(PlayerController playerController, float suspendPhysicsTime)
    : base(playerController, suspendPhysicsTime)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.red;

    playerController.PlayerState |= PlayerState.Invincible;

    _suspendPhysicsEndTime = Time.time + suspendPhysicsTime;
  }

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.Invincible;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (Time.time <= _suspendPhysicsEndTime)
    {
      return ControlHandlerAfterUpdateStatus.KeepAlive;
    }

    return base.DoUpdate();
  }
}
