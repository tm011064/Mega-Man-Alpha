using UnityEngine;

public class CheckpointPortal : MonoBehaviour
{
  public bool MoveToNextCheckpointIndex = true;

  public int MoveToCheckpointIndex = 0;

  void OnTriggerEnter2D(Collider2D col)
  {
    if (MoveToNextCheckpointIndex)
    {
      GameManager.Instance.SpawnPlayerAtNextCheckpoint(false);
    }
    else
    {
      GameManager.Instance.SpawnPlayerAtCheckpoint(MoveToCheckpointIndex);
    }
  }
}
