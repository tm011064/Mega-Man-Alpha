using System;
using UnityEngine;

public partial class EdgeColliderTriggerEnterBehaviour : ColliderTriggerEnterBehaviour, ITriggerEnterExit
{
  protected override void InvokeHandler(EventHandler<TriggerEnterExitEventArgs> handler, Collider2D collider)
  {
    var edgeCollider = this.GetComponentOrThrow<EdgeCollider2D>();

    handler(this, new TriggerEnterExitEventArgs(edgeCollider, collider));
  }
}
