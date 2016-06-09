public class JumpingRunnerEnemyController : TopBounceableEnemyController
{
  public float Speed = 200f;

  public float Gravity = -3960f;

  public float JumpHeight = 256f;

  public float JumpInterval = 2f;

  protected override BaseControlHandler ApplyDamageControlHandler
  {
    get { return new DamageTakenPlayerControlHandler(); }
  }

  public override void Reset(Direction startDirection)
  {
    ResetControlHandlers(new JumpingRunnerEnemyControlHandler(this, startDirection));
  }
}
