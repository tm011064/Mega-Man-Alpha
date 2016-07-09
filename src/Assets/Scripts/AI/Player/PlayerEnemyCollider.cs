using UnityEngine;

public class PlayerEnemyCollider : MonoBehaviour
{
  private PlayerController _playerController;

  void Start()
  {
    _playerController = GameManager.Instance.Player;
  }

  private void HandlePlayerCollision(Collider2D collider)
  {
    var enemyController = collider.gameObject.GetComponent<IPlayerCollidable>();

    if (enemyController != null)
    {
      enemyController.OnPlayerCollide(_playerController);
    }
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    HandlePlayerCollision(collider);
  }

  void OnTriggerStay2D(Collider2D collider)
  {
    if ((_playerController.PlayerState & PlayerState.Invincible) != 0)
    {
      return;
    }

    HandlePlayerCollision(collider);
  }
}
