using UnityEngine;

public class BallisticTrajectoryControlHandler : BaseControlHandler
{
  private float _gravity;

  private Vector3 _velocity;

  private ControlHandlerAfterUpdateStatus _controlHandlerUpdateStatus = ControlHandlerAfterUpdateStatus.KeepAlive;

  public BallisticTrajectoryControlHandler(
    CharacterPhysicsManager characterPhysicsManager,
    Vector3 startPosition,
    Vector3 endPosition,
    float gravity,
    float angle)
    : base(characterPhysicsManager)
  {
    SetDebugDraw(Color.cyan, true);

    if (characterPhysicsManager != null)
    {
      characterPhysicsManager.ControllerBecameGrounded += 
        _ => _controlHandlerUpdateStatus = ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    _gravity = gravity;

    _velocity = DynamicsUtility.GetBallisticVelocity(endPosition, startPosition, angle, _gravity);

    Logger.Trace("Ballistic start velocity: " + _velocity + ", (startPosition: " + startPosition + ", endPosition: " + endPosition + ", gravity: " + gravity + ", angle: " + angle + ")");
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    _velocity.y += _gravity * Time.deltaTime;

    CharacterPhysicsManager.Move(_velocity * Time.deltaTime);

    return _controlHandlerUpdateStatus;
  }
}
