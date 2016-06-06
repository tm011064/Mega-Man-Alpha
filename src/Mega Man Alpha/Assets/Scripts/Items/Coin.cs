using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D col)
  {
    GameManager.Instance.AddCoin();

    Destroy(gameObject);
  }
}
