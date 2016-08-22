using UnityEngine;

public class PlayerProjectileBehaviour : MonoBehaviour
{
  public int DamageUnits = 1;

  [Tooltip("Specifies what happens to a projectile if an enemy blocks the shot")]
  public ProjectileBlockedBehaviour ProjectileBlockedBehaviour = ProjectileBlockedBehaviour.Disappear;

  private Vector3 _velocity;

  private IProjectileReboundBehaviour _projectileReboundBehaviour;

  void Awake()
  {
    if (ProjectileBlockedBehaviour == ProjectileBlockedBehaviour.Rebound)
    {
      _projectileReboundBehaviour = this.GetComponentOrThrow<IProjectileReboundBehaviour>();
    }
  }

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
    var enemyHealthBehaviour = collider.GetComponentOrThrow<EnemyHealthBehaviour>();

    var damageResult = enemyHealthBehaviour.ApplyDamage(DamageUnits);

    if (damageResult == DamageResult.Invincible
      && ProjectileBlockedBehaviour == ProjectileBlockedBehaviour.Rebound)
    {
      _velocity = _projectileReboundBehaviour.HandleRebound(
        GetComponent<BoxCollider2D>(),
        collider,
        _velocity);

      return;
    }

    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }
}
