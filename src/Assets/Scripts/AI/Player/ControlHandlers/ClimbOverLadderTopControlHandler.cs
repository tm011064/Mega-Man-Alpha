using UnityEngine;

public class ClimbOverLadderTopControlHandler : PlayerControlHandler
{
  private readonly Vector2 _collisionExtents;

  private readonly Transform _transform;

  public ClimbOverLadderTopControlHandler(
    PlayerController playerController,
    Transform transform,
    Vector2 collisionExtents)
    : base(
      playerController,
      new PlayerStateController[] { new ClimbController(playerController) })
  {
    SetDebugDraw(Color.green, true);

    _collisionExtents = collisionExtents;
    _transform = transform;
  }

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.ClimbingLadderTop;
    PlayerController.PlayerState &= ~PlayerState.ClimbingLadder;

    CharacterPhysicsManager.WarpToGrounded();
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.ClimbingLadderTop;

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (PlayerController.EnvironmentBoxCollider.bounds.min.y > _transform.position.y + _collisionExtents.y + .1f)
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
