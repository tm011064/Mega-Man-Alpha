using System;
using UnityEngine;

public class TriggerEnterExitEventArgs : EventArgs
{
  public Collider2D SourceCollider { get; private set; }

  public Collider2D OtherCollider { get; private set; }

  public TriggerEnterExitEventArgs(Collider2D sourceCollider, Collider2D otherCollider)
  {
    SourceCollider = sourceCollider;
    OtherCollider = otherCollider;
  }
}
