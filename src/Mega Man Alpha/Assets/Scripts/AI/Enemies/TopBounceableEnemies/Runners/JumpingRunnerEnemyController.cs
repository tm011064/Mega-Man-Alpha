public class JumpingRunnerEnemyController : TopBounceableEnemyController
{
  public float speed = 200f;

  public float gravity = -3960f;

  public float jumpHeight = 256f;

  public float jumpInterval = 2f;

  protected override BaseControlHandler ApplyDamageControlHandler
  {
    get { return new DamageTakenPlayerControlHandler(); }
  }

  public override void Reset(Direction startDirection)
  {
    ResetControlHandlers(new JumpingRunnerEnemyControlHandler(this, startDirection));
  }
}
