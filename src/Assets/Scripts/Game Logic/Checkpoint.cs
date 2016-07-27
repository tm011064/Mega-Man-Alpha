using UnityEngine;

public partial class Checkpoint : MonoBehaviour
{
  public int Index = 0;

  void OnTriggerEnter2D(Collider2D collider)
  {
    GameManager.Instance.Player.SpawnLocation = transform.position;
  }
}
