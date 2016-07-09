using UnityEngine;

public class EnemyDeathAnimationBehaviour : MonoBehaviour
{
  public void Deactivate()
  {
    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }
}
