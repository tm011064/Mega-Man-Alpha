using UnityEngine;

public class ApplyDamageHazard : MonoBehaviour
{
  public bool DestroyHazardOnCollision = false;

  private GameManager _gameManager;

  void OnTriggerStay2D(Collider2D col)
  {
    if (col.gameObject == _gameManager.Player.gameObject)
    {
      if (_gameManager.Player.IsInvincible)
      {
        return;
      }

      if (DestroyHazardOnCollision)
      {
        ObjectPoolingManager.Instance.Deactivate(gameObject);
      }

      switch (_gameManager.PowerUpManager.AddDamage())
      {
        case PowerUpManager.DamageResult.IsDead: return;

        default:

          _gameManager.Player.PushControlHandler(new DamageTakenPlayerControlHandler());

          break;
      }
    }
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    // we have to check for player as the hazard might have collided with a hazard destroy trigger
    if (col.gameObject == _gameManager.Player.gameObject)
    {
      if (_gameManager.Player.IsInvincible)
      {
        return;
      }

      if (DestroyHazardOnCollision)
      {
        ObjectPoolingManager.Instance.Deactivate(gameObject);
      }

      switch (_gameManager.PowerUpManager.AddDamage())
      {
        case PowerUpManager.DamageResult.IsDead: return;

        default:

          _gameManager.Player.PushControlHandler(new DamageTakenPlayerControlHandler());

          break;
      }
    }
  }

  void Awake()
  {
    _gameManager = GameManager.Instance;
  }
}
