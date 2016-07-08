using UnityEngine;

public class PlayerProjectileBehaviour : MonoBehaviour
{
  public int DamageUnits = 1;

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
    ObjectPoolingManager.Instance.Deactivate(gameObject);

    if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
      var enemyHealthBehaviour = collider.GetComponent<EnemyHealthBehaviour>();

      if (enemyHealthBehaviour == null)
      {
        throw new MissingComponentException("Object " + name + " has no 'Enemy Health Behaviour' script component");
      }

      enemyHealthBehaviour.ApplyDamage(DamageUnits);
    }
  }
}
