public class StationaryEnemyController : TopBounceableEnemyController
{
  protected override BaseControlHandler ApplyDamageControlHandler
  {
    get { return new DamageTakenPlayerControlHandler(); }
  }

  public override void Reset(Direction startDirection)
  {
    ResetControlHandlers(new StationaryEnemyControlHandler());
  }
}
