using System;
using UnityEngine;

public class StartClimbDownLadderControlHandler : PlayerControlHandler
{
  private readonly Vector2 _collisionExtents;

  private readonly float _ladderTopAnimationDistance;

  private readonly Transform _transform;

  public StartClimbDownLadderControlHandler(
    PlayerController playerController,
    Transform transform,
    Vector2 collisionExtents,
    float ladderTopAnimationDistance)
    : base(playerController, new PlayerStateController[] { new ClimbController(playerController) })
  {
    SetDebugDraw(Color.green, true);

    _ladderTopAnimationDistance = ladderTopAnimationDistance;
    _collisionExtents = collisionExtents;
    _transform = transform;
  }

  public event Action<StartClimbDownLadderControlHandler> Disposed;

  public override void Dispose()
  {
    var handler = Disposed;

    if (handler != null)
    {
      Disposed(this);
    }
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (PlayerController.EnvironmentBoxCollider.bounds.max.y < _transform.position.y + _collisionExtents.y + _ladderTopAnimationDistance)
    {
      GameManager.Player.InsertControlHandlerBeforeCurrent(
        new LadderClimbControlHandler(
          PlayerController,
          _transform,
          _collisionExtents,
          _ladderTopAnimationDistance));

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var velocity = new Vector3(
      0f,
      PlayerController.ClimbSettings.ClimbDownVelocity);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
