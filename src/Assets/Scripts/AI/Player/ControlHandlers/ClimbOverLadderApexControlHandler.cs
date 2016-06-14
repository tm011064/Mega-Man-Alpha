using UnityEngine;

public class ClimbOverLadderApexControlHandler : PlayerControlHandler
{
  private readonly float _targetPositionY;

  public ClimbOverLadderApexControlHandler(PlayerController playerController, float targetPositionY)
    : base(playerController)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.green;

    _targetPositionY = targetPositionY;
  }

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (PlayerController.BoxCollider.bounds.center.y - PlayerController.BoxCollider.bounds.extents.y > _targetPositionY)
    {
      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var velocity = new Vector3(
      0f,
      PlayerController.ClimbSettings.ClimbUpVelocity);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
