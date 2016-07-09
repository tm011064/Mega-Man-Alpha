using UnityEngine;

public class EnemyRocket : MonoBehaviour, IEnemyProjectile
{
  private bool _hasStarted = false;

  private float _acceleration;

  private float _targetVelocity;

  private Vector2 _direction;

  private Vector2 _velocity;

  void OnBecameInvisible()
  {
    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    ObjectPoolingManager.Instance.Deactivate(gameObject);

    if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
    {
      GameManager.Instance.Player.OnPlayerDied();
    }
  }

  void Update()
  {
    if (_hasStarted)
    {
      _velocity = _velocity + (_direction * _acceleration * Time.deltaTime);

      if (_velocity.magnitude > _targetVelocity)
      {
        _velocity = _direction * _targetVelocity;
      }

      transform.Translate(_velocity);
    }
  }

  public void StartMove(Vector2 startPosition, Vector2 direction, float acceleration, float targetVelocity)
  {
    transform.position = startPosition;

    _hasStarted = true;

    _direction = direction.normalized;

    _acceleration = acceleration;

    _targetVelocity = targetVelocity;

    _velocity = Vector2.zero;
  }
}
