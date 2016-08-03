using UnityEngine;

public class BarrelMetBarrelRollControlHandler : EnemyControlHandler<BarrelMetBarrel>
{
  private float _moveDirectionFactor;

  public BarrelMetBarrelRollControlHandler(BarrelMetBarrel enemyController, Direction startDirection)
    : base(enemyController, -1f, enemyController.GetComponent<Animator>())
  {
    _moveDirectionFactor = startDirection == Direction.Left
      ? -1f
      : 1f;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    PlayAnimation(Animator.StringToHash("Move"));

    MoveHorizontally(
      ref _moveDirectionFactor,
      _enemyController.Speed,
      _enemyController.Gravity,
      PlatformEdgeMoveMode.FallOff);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
