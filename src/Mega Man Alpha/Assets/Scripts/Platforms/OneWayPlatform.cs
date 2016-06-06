using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OneWayPlatform : MonoBehaviour
{
  Collider2D _collider;

  private void ResetLayer()
  {
    _collider.enabled = true;
  }

  public void TriggerFall()
  {
    _collider.enabled = false;

    Invoke("ResetLayer", .2f);
  }

  void Awake()
  {
    _collider = GetComponent<Collider2D>();
  }
}
