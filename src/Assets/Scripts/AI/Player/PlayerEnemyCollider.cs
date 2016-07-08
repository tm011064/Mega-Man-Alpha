using UnityEngine;

public class PlayerEnemyCollider : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
      var enemyController = collider.gameObject.GetComponent<IPlayerCollidable>();

      if (enemyController != null)
      {
        enemyController.OnPlayerCollide(GameManager.Instance.Player);
      }
    }
  }
}
