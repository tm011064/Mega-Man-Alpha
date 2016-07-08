using UnityEngine;

public class BarrelMetBarrelIdleControlHandler : EnemyControlHandler<BarrelMetBarrel>
{
  public BarrelMetBarrelIdleControlHandler(BarrelMetBarrel enemyController, Direction startDirection)
    : base(enemyController, -1f, enemyController.GetComponent<Animator>())
  {
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    PlayAnimation(Animator.StringToHash("Idle"));

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
