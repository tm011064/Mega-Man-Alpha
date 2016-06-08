using UnityEngine;

public class Bullet : MonoBehaviour
{
  private Rigidbody2D _rigidBody;

  private Vector3 _velocity;

  void Awake()
  {
    _rigidBody = GetComponent<Rigidbody2D>();
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

  void OnTriggerEnter2D(Collider2D col)
  {
    // TODO (Roman): bullet going up should go through one way platform, but hit when going down - just as player does
    ObjectPoolingManager.Instance.Deactivate(gameObject);

    Debug.Log("Collided with " + col.gameObject.name);

    if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
      // TODO (Roman): this should be somewhere else - this is just test code
      var deathParticles = ObjectPoolingManager.Instance.GetObject(
        GameManager.Instance.GameSettings.PooledObjects.DefaultEnemyDeathParticlePrefab.Prefab.name);

      deathParticles.transform.position = col.gameObject.transform.position;

      ObjectPoolingManager.Instance.Deactivate(col.gameObject);

      Debug.Log("Collided with enemy " + col.gameObject.name);
    }
  }
}
