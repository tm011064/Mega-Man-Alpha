using System;
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

    if (GameManager.Instance.Player.State.Is(PlayerStatesNeededToEnter))
    {
      _playerColliderState &= ~PlayerColliderState.WrongStateOnEnter;

      InvokeEnter(collider);
    }
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    _playerColliderState = PlayerColliderState.Inside;

    if (PlayerStatesNeededToEnter != null
      && !GameManager.Instance.Player.State.Is(PlayerStatesNeededToEnter))
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
