using UnityEngine;

public abstract class TopBounceableEnemyController : EnemyController
{
  [Tooltip("If the player does not press the jump button when hitting the enemy, this multiplier is used to calculate the bounce height.")]
  public float BounceJumpMultiplier = .5f;

  [Tooltip("Specifies how much time the player has to perform a jump after colliding with this enemy.")]
  public float AllowJumpFromTopThresholdInSeconds = .3f;

  [Tooltip("-45 is the top left corner")]
  public float AllowedTopCollisionAngleFrom = -38f;

  [Tooltip("-135 is the top right corner")]
  public float AllowedTopCollisionAngleTo = -142f;

  protected abstract BaseControlHandler ApplyDamageControlHandler { get; }

  public override void OnPlayerCollide(PlayerController playerController)
  {
    var direction = transform.position - playerController.transform.position;

    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    Logger.Info("Player collided with " + name + " enemy. Player Position: " + playerController.transform.position + ", Enemy position: " + transform.position + ",  Angle: " + angle);

    if (angle < AllowedTopCollisionAngleFrom
      && angle > AllowedTopCollisionAngleTo)
    {
      playerController.PushControlHandler(
        new TopBounceableControlHandler(playerController, AllowJumpFromTopThresholdInSeconds, BounceJumpMultiplier));

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

