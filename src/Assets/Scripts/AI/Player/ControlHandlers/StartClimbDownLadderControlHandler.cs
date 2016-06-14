using System;
using UnityEngine;

public class StartClimbDownLadderControlHandler : PlayerControlHandler
{
  private readonly Bounds _ladderArea;

  private readonly float _climbOverTopExtension;

  public StartClimbDownLadderControlHandler(
    PlayerController playerController,
    Bounds ladderArea,
    float climbOverTopExtension)
    : base(playerController)
  {
    DoDrawDebugBoundingBox = true;
    DebugBoundingBoxColor = Color.green;

    _ladderArea = ladderArea;
    _climbOverTopExtension = climbOverTopExtension;
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
      + PlayerController.BoxCollider.bounds.extents.y < _climbOverTopExtension)
    {
      GameManager.Player.InsertControlHandlerBeforeCurrent(
        new LadderClimbControlHandler(PlayerController, _ladderArea, _climbOverTopExtension));

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var velocity = new Vector3(
      0f,
      PlayerController.ClimbSettings.ClimbDownVelocity); // TODO (Roman): get from settings

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
