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
      HandleProjectileRebound(collider);

      return;
    }

    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }

  private void MoveProjectileOutsideOfEnemyCollider(Collider2D enemyCollider)
  {
    var projectileCollider = GetComponent<BoxCollider2D>();

    var deltaX = 0f;
    var deltaY = 0f;
    
    // TODO (Roman): this only really works for megabuster, rebound handling must be specified by weapon itself
    // for better results
    if (_velocity.x > 0f)
    {
      deltaX = enemyCollider.bounds.min.x - projectileCollider.bounds.extents.x - projectileCollider.bounds.center.x;
    }
    else if (_velocity.x < 0f)
    {
      deltaX = enemyCollider.bounds.max.x + projectileCollider.bounds.extents.x - projectileCollider.bounds.center.x;
    }

    //if (_velocity.y > 0f)
    //{
    //  deltaY = enemyCollider.bounds.min.y - projectileCollider.bounds.extents.y - projectileCollider.bounds.center.y;
    //}
    //else if (_velocity.y < 0f)
    //{
    //  deltaY = enemyCollider.bounds.max.y + projectileCollider.bounds.extents.y - enemyCollider.bounds.max.y;
    //}

    var vector = new Vector2(deltaX, deltaY) * 1.001f; // smudge factor

    transform.Translate(vector, Space.World);
  }

  private void HandleProjectileRebound(Collider2D enemyCollider)
  {
    MoveProjectileOutsideOfEnemyCollider(enemyCollider);

    var zRotationAngle = (_velocity.x > 0f) ? 135f : -135f;

    _velocity = Quaternion.Euler(0, 0, zRotationAngle) * _velocity;

    Debug.Log(Time.time + " -> v: " + _velocity);
  }
}
