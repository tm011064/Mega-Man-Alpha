public class FlyingFollowerEnemyController : EnemyController
{
  public float speed = 100f;

  public float smoothDampFactor = 2.5f;

  public override void Reset(Direction startDirection)
  {
    ResetControlHandlers(new FlyingFollowerEnemyControlHandler(this));
  }
}
