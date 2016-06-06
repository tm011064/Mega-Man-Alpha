using UnityEngine;

public abstract class TopBounceableEnemyController : EnemyController
{
  [Tooltip("If the player does not press the jump button when hitting the enemy, this multiplier is used to calculate the bounce height.")]
  public float bounceJumpMultiplier = .5f;

  [Tooltip("Specifies how much time the player has to perform a jump after colliding with this enemy.")]
  public float allowJumpFromTopThresholdInSeconds = .3f;

  [Tooltip("-45 is the top left corner")]
  public float allowedTopCollisionAngleFrom = -38f;

  [Tooltip("-135 is the top right corner")]
  public float allowedTopCollisionAngleTo = -142f;

  protected abstract BaseControlHandler ApplyDamageControlHandler { get; }

  public override void OnPlayerCollide(PlayerController playerController)
  {
    var dir = transform.position - playerController.transform.position;

    var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    Logger.Info("Player collided with " + name + " enemy. Player Position: " + playerController.transform.position + ", Enemy position: " + transform.position + ",  Angle: " + angle);

    if (angle < allowedTopCollisionAngleFrom
      && angle > allowedTopCollisionAngleTo)
    {
      playerController.PushControlHandler(
        new TopBounceableControlHandler(playerController, allowJumpFromTopThresholdInSeconds, bounceJumpMultiplier));

      // TODO (Roman): run death animation...
      ObjectPoolingManager.Instance.Deactivate(gameObject);
    }
    else
    {
      if (playerController.IsInvincible)
      {
        return;
      }

      switch (GameManager.Instance.PowerUpManager.AddDamage())
      {
        case PowerUpManager.DamageResult.IsDead: break;

        default:
          playerController.PushControlHandler(this.ApplyDamageControlHandler);
          break;
      }
    }
  }

}

