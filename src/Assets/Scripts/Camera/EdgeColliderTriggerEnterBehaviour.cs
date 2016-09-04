using System;
using System.Linq;
using UnityEngine;

public partial class EdgeColliderTriggerEnterBehaviour : MonoBehaviour, ITriggerEnterExit
{
  public PlayerState[] PlayerStatesNeededToEnter;

  public event EventHandler<TriggerEnterExitEventArgs> Entered;

  public event EventHandler<TriggerEnterExitEventArgs> Exited;

  void OnTriggerEnter2D(Collider2D collider)
  {
    var playerState = GameManager.Instance.Player.PlayerState;

    if (PlayerStatesNeededToEnter != null
      && PlayerStatesNeededToEnter.Any(ps => (playerState & ps) == 0))
    {
      return;
    }

    var handler = Entered;

    if (handler != null)
    {
      InvokeHandler(handler, collider);
    }
  }

  void OnTriggerExit2D(Collider2D collider)
  {
    var handler = Exited;

    if (handler != null)
    {
      InvokeHandler(handler, collider);
    }
  }

  private void InvokeHandler(EventHandler<TriggerEnterExitEventArgs> handler, Collider2D collider)
  {
    var edgeCollider = this.GetComponentOrThrow<EdgeCollider2D>();

    handler(this, new TriggerEnterExitEventArgs(edgeCollider, collider));
  }
}
