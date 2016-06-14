using System;
using UnityEngine;

public class StartClimbDownLadderControlHandler : PlayerControlHandler
{
  private readonly Bounds _ladderArea;

  private readonly float _topEdgePositionY;

  public StartClimbDownLadderControlHandler(
    PlayerController playerController,
    Bounds ladderArea,
    float topEdgePositionY)
    : base(playerController)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.green;

    _ladderArea = ladderArea;
    _topEdgePositionY = topEdgePositionY;
  }

  public event Action<StartClimbDownLadderControlHandler> Disposed;

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

    var handler = Disposed;

    if (handler != null)
    {
      Disposed(this);
    }
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (PlayerController.BoxCollider.bounds.center.y
      + PlayerController.BoxCollider.bounds.extents.y < _topEdgePositionY)
    {
      GameManager.Player.InsertControlHandlerBeforeCurrent(
        new LadderClimbControlHandler(PlayerController, _ladderArea, _topEdgePositionY));

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var velocity = new Vector3(
      0f,
      PlayerController.ClimbSettings.ClimbDownVelocity);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
