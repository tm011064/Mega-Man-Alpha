public class RunnerEnemyControlHandler : EnemyControlHandler<RunnerEnemyController>
{
  private float _moveDirectionFactor;

  public RunnerEnemyControlHandler(RunnerEnemyController patrollerEnemyController, Direction startDirection)
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
      PlatformEdgeMoveMode.FallOff);

    return true;
  }
}
