using System;
using UnityEngine;

public class ApplyDamageHazard : MonoBehaviour
{
  public bool DestroyHazardOnCollision = false;

  private GameManager _gameManager;

  void OnTriggerStay2D(Collider2D col)
  {
    if (col.gameObject == _gameManager.Player.gameObject)
    {
      if ((_gameManager.Player.PlayerState & PlayerState.Invincible) != 0)
      {
        return;
      }

      if (DestroyHazardOnCollision)
      {
        ObjectPoolingManager.Instance.Deactivate(gameObject);
      }

      // TODO (Roman): add player damage logic here
      throw new NotImplementedException("Player damage not implemented");
    }
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    // we have to check for player as the hazard might have collided with a hazard destroy trigger
    if (col.gameObject == _gameManager.Player.gameObject)
    {
      if ((_gameManager.Player.PlayerState & PlayerState.Invincible) != 0)
      {
        return;
      }

      if (DestroyHazardOnCollision)
      {
        ObjectPoolingManager.Instance.Deactivate(gameObject);
      }

      // TODO (Roman): add player damage logic here
      throw new NotImplementedException("Player damage not implemented");
    }
  }

  void Awake()
  {
    _gameManager = GameManager.Instance;
  }
}
