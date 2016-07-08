public class EnemyContactKnockbackPlayerStateController : PlayerStateController
{
  public EnemyContactKnockbackPlayerStateController(PlayerController playerController)
    : base(playerController)
  {
  }

  public override PlayerStateUpdateResult GetPlayerStateUpdateResult(XYAxisState axisState)
  {
    return PlayerStateUpdateResult.CreateHandled("Enemy Contact Knockback");
  }
}
