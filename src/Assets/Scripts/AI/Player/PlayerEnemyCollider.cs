using UnityEngine;

public class PlayerEnemyCollider : MonoBehaviour
{
  private PlayerController _playerController;

  void Awake()
  {
    _playerController = GameManager.Instance.Player;
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
      var enemyController = collider.gameObject.GetComponent<EnemyController>();

      if (enemyController != null)
      {
        enemyController.OnPlayerCollide(_playerController);
      }
    }
  }
}
