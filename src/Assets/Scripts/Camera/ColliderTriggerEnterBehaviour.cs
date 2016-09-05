using System;
using System.Linq;
using UnityEngine;

public abstract class ColliderTriggerEnterBehaviour : MonoBehaviour
{
  private PlayerColliderState _playerColliderState;

  public PlayerState[] PlayerStatesNeededToEnter;

  public event EventHandler<TriggerEnterExitEventArgs> Entered;

  public event EventHandler<TriggerEnterExitEventArgs> Exited;

  protected abstract void InvokeHandler(EventHandler<TriggerEnterExitEventArgs> handler, Collider2D collider);

  private void InvokeEnter(Collider2D collider)
  {
    var handler = Entered;

    if (handler != null)
    {
      InvokeHandler(handler, collider);
    }
  }

  void OnTriggerStay2D(Collider2D collider)
  {
    if ((_playerColliderState & PlayerColliderState.WrongStateOnEnter) == 0)
    {
      return;
    }

    var playerState = GameManager.Instance.Player.PlayerState;

    if (PlayerStatesNeededToEnter.All(ps => (playerState & ps) != 0))
    {
      _playerColliderState &= ~PlayerColliderState.WrongStateOnEnter;

      InvokeEnter(collider);
    }
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    _playerColliderState = PlayerColliderState.Inside;

    var playerState = GameManager.Instance.Player.PlayerState;

    if (PlayerStatesNeededToEnter != null
      && PlayerStatesNeededToEnter.Any(ps => (playerState & ps) == 0))
    {
      _playerColliderState |= PlayerColliderState.WrongStateOnEnter;

      return;
    }

    InvokeEnter(collider);
  }

  void OnTriggerExit2D(Collider2D collider)
  {
    _playerColliderState = PlayerColliderState.Outside;

    var handler = Exited;

    if (handler != null)
    {
      InvokeHandler(handler, collider);
    }
  }

  private enum PlayerColliderState
  {
    Outside,
    Inside,
    WrongStateOnEnter
  }
}
