using UnityEngine;

public class PlayerProjectileBehaviour : MonoBehaviour
{
  public int DamageUnits = 1;

  [Tooltip("Specifies what happens to a projectile if an enemy blocks the shot")]
  public ProjectileBlockedBehaviour ProjectileBlockedBehaviour = ProjectileBlockedBehaviour.Disappear;

  private Vector3 _velocity;

  public void StartMove(Vector2 startPosition, Vector2 velocity)
  {
    transform.position = startPosition;

    _velocity = velocity.ToVector3();
  }

  void Update()
  {
    transform.Translate(_velocity * Time.deltaTime, Space.World);
  }

  void OnBecameInvisible()
  {
    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
      HandleEnemyCollision(collider);

      return;
    }

    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }

  private void HandleEnemyCollision(Collider2D collider)
  {
    var enemyHealthBehaviour = collider.GetComponent<EnemyHealthBehaviour>();

    if (enemyHealthBehaviour == null)
    {
      throw new MissingComponentException("Object " + name + " has no 'Enemy Health Behaviour' script component");
    }

    var damageResult = enemyHealthBehaviour.ApplyDamage(DamageUnits);

    if (damageResult == DamageResult.Invincible
      && ProjectileBlockedBehaviour == ProjectileBlockedBehaviour.Rebound)
    {
      HandleProjectileRebound();

      return;
    }

    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }

  private void HandleProjectileRebound()
  {
    // first undo the last translation so we are outside the bounding box
    transform.Translate(_velocity * -Time.deltaTime, Space.World);

    var zRotationAngle = (_velocity.x > 0f) ? 135f : -135f;

    _velocity = Quaternion.Euler(0, 0, zRotationAngle) * _velocity;
  }
}
