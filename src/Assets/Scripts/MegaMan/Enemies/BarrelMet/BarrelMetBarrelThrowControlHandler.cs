using UnityEngine;

public class BarrelMetBarrelThrowControlHandler : EnemyControlHandler<BarrelMetBarrel>
{
  private readonly BallisticTrajectorySettings _ballisticTrajectorySettings;

  private Vector2 _velocity;

  private int _isFirstUpdate;

  private float _moveDirectionFactor;

  public BarrelMetBarrelThrowControlHandler(
    BarrelMetBarrel enemyController,
    Direction startDirection,
    BallisticTrajectorySettings ballisticTrajectorySettings)
    : base(enemyController, -1f, enemyController.GetComponent<Animator>())
  {
    _moveDirectionFactor = startDirection == Direction.Left
      ? -1f
      : 1f;

    _ballisticTrajectorySettings = ballisticTrajectorySettings;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    var targetPosition = _enemyController.gameObject.transform.position
      + new Vector3(
          _ballisticTrajectorySettings.EndPosition.x * _moveDirectionFactor,
          _ballisticTrajectorySettings.EndPosition.y,
          _enemyController.gameObject.transform.position.z);

    _velocity = DynamicsUtility.GetBallisticVelocity(
      targetPosition,
      _enemyController.gameObject.transform.position,
      _ballisticTrajectorySettings.Angle,
      _ballisticTrajectorySettings.ProjectileGravity);

    return true;
  }

  private bool IsCloseToFloor()
  {
    CharacterPhysicsManager.PrimeRaycastOrigins();

    return CharacterPhysicsManager.IsFloorWithinDistance(.1f);
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (_velocity.y < 0f
      && IsCloseToFloor())
    {
      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    PlayAnimation(Animator.StringToHash("Move"));

    _velocity.y += _ballisticTrajectorySettings.ProjectileGravity * Time.deltaTime;

    _enemyController.gameObject.transform.Translate(_velocity * Time.deltaTime, Space.World);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
