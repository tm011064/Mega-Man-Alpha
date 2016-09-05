using System;
using UnityEngine;

public partial class BoxColliderTriggerEnterBehaviour : ColliderTriggerEnterBehaviour, ITriggerEnterExit
{
  protected override void InvokeHandler(EventHandler<TriggerEnterExitEventArgs> handler, Collider2D collider)
  {
    var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();

    handler(this, new TriggerEnterExitEventArgs(boxCollider, collider));
  }
}
