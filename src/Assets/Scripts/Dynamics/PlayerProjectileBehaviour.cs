using UnityEngine;

public class PlayerProjectileBehaviour : MonoBehaviour
{
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

  void OnTriggerEnter2D(Collider2D col)
  {
    ObjectPoolingManager.Instance.Deactivate(gameObject);

    if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
      ObjectPoolingManager.Instance.Deactivate(col.gameObject);
    }
  }
}
