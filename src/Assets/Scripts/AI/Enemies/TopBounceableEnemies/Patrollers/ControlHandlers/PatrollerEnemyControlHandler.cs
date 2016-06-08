public class PatrollerEnemyControlHandler : EnemyControlHandler<PatrollerEnemyController>
{
  private float _moveDirectionFactor;

  public PatrollerEnemyControlHandler(PatrollerEnemyController patrollerEnemyController, Direction startDirection)
    : base(patrollerEnemyController, -1f)
  {
    _moveDirectionFactor = startDirection == Direction.Left
      ? -1f
      : 1f;
  }

  protected override bool DoUpdate()
  {
    // first move in patrolling mode
    MoveHorizontally(
      ref _moveDirectionFactor,
      _enemyController.speed,
      _enemyController.gravity,
      PlatformEdgeMoveMode.TurnAround);

    return true;
  }
}
