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
    MoveHorizontally(
      ref _moveDirectionFactor,
      _enemyController.Speed,
      _enemyController.Gravity,
      PlatformEdgeMoveMode.TurnAround);

    return true;
  }
}
