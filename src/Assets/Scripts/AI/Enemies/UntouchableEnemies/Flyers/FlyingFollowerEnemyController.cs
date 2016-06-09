public class FlyingFollowerEnemyController : EnemyController
{
  public float Speed = 100f;

  public float SmoothDampFactor = 2.5f;

  public override void Reset(Direction startDirection)
  {
    ResetControlHandlers(new FlyingFollowerEnemyControlHandler(this));
  }
}
