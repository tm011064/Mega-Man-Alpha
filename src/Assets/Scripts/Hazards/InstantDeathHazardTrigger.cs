using UnityEngine;

public class InstantDeathHazardTrigger : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D col)
  {
    GameManager.Instance.Player.OnPlayerDied();
  }
}
