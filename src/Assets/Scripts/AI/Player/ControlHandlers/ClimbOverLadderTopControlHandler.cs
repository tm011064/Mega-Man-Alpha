using UnityEngine;

public class ClimbOverLadderTopControlHandler : PlayerControlHandler
{
  private readonly float _ladderTopEdgePosY;

  public ClimbOverLadderTopControlHandler(PlayerController playerController, float ladderTopEdgePosY)
    : base(
      playerController,
      new PlayerStateController[] { new ClimbController(playerController) })
  {
    SetDebugDraw(Color.green, true);

    _ladderTopEdgePosY = ladderTopEdgePosY;
  }

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.ClimbingLadderTop;
    PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

    PlayerController.transform.position = new Vector3(
      PlayerController.transform.position.x,
      _ladderTopEdgePosY + PlayerController.transform.position.y - PlayerController.BoxCollider.bounds.min.y,
      PlayerController.transform.position.z);
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.ClimbingLadderTop;

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (PlayerController.BoxCollider.bounds.min.y > _ladderTopEdgePosY)
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
