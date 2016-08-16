using System;
using UnityEngine;

public partial class EdgeColliderTriggerEnterBehaviour : MonoBehaviour, ITriggerEnter
{
  public event Action Entered;

  void OnTriggerEnter2D(Collider2D collider)
  {
    var handler = Entered;

    if (handler != null)
    {
      handler();
    }
  }
}
