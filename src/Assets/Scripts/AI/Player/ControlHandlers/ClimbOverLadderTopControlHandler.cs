using UnityEngine;

public class ClimbOverLadderTopControlHandler : PlayerControlHandler
{
  private readonly float _topEdgePositionY;

  public ClimbOverLadderTopControlHandler(PlayerController playerController, float topEdgePositionY)
    : base(playerController)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.green;

    _topEdgePositionY = topEdgePositionY;
  }

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.ClimbingLadderTop;
    PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= ~PlayerState.ClimbingLadderTop;

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (PlayerController.BoxCollider.bounds.center.y
      - PlayerController.BoxCollider.bounds.extents.y > _topEdgePositionY)
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
