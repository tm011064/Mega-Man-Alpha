using UnityEngine;

public class SpinMeleeAttack : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D col)
  {
    Debug.Log("Spin Melee Collided with " + col.gameObject.name);

    ObjectPoolingManager.Instance.Deactivate(col.gameObject);
  }
}
