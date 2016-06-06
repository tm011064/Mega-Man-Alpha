using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
  public PowerUpType PowerUpType = PowerUpType.Basic;

  void OnTriggerEnter2D(Collider2D col)
  {
    // we know from the layer mask arrangement that this trigger can only be called by the player object
    GameManager.Instance.PowerUpManager.ApplyPowerUpItem(PowerUpType);

    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }
}
