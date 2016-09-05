using System;
using UnityEngine;

/// <summary>
/// A character controller controls all of a character's behavior. It is responsible for managing a
/// character's <see cref="BaseControlHandler"/>s and perform game loop updates. 
/// </summary>
public class BaseCharacterController : BaseMonoBehaviour
{
  [HideInInspector]
  public CharacterPhysicsManager CharacterPhysicsManager;

  private CustomStack<BaseControlHandler> _controlHandlers = new CustomStack<BaseControlHandler>();

  private BaseControlHandler _activeControlHandler = null;

  public BaseControlHandler ActiveControlHandler { get { return _activeControlHandler; } }

  private void TryActivateCurrentControlHandler(BaseControlHandler previousControlHandler)
  {
    _activeControlHandler = _controlHandlers.Peek();

    while (_activeControlHandler != null
      && !_activeControlHandler.TryActivate(previousControlHandler))
    {
      previousControlHandler = _controlHandlers.Pop();

      Logger.Info("Popped handler: " + previousControlHandler.ToString());

      previousControlHandler.Dispose();

      _activeControlHandler = _controlHandlers.Peek();
    }
  }

  protected virtual void Update()
  {
    try
    {
      if (_activeControlHandler != null)
      {
        while (_activeControlHandler.Update() == ControlHandlerAfterUpdateStatus.CanBeDisposed)
        {
          var poppedHandler = _controlHandlers.Pop();

          poppedHandler.Dispose();

          Logger.Info("Popped handler: " + poppedHandler.ToString());

          TryActivateCurrentControlHandler(poppedHandler);
        }

        // after we updated the control handler, we now want to notify all stack members (excluding the current handler/peek) that an update
        // has occurred. This is necessary in case stacked handlers need to react to actions, for example: melee attack is interrupted by wall jump handler
        for (var i = _controlHandlers.Count - 2; i >= 0; i--)
        {
          _controlHandlers[i].OnAfterStackPeekUpdate();
        }
      }
    }
    catch (Exception err)
    {
      Logger.Error("Game object " + name + " misses default control handler.", err);

      throw;
    }
  }

  public void ResetControlHandlers(BaseControlHandler controlHandler = null)
  {
    Logger.Info("Resetting character control handlers.");

    for (var i = _controlHandlers.Count - 1; i >= 0; i--)
    {
      Logger.Info("Removing handler: " + _controlHandlers[i].ToString());

      _controlHandlers[i].Dispose();

      _controlHandlers.RemoveAt(i);
    }

    _activeControlHandler = null;

    if (controlHandler != null)
    {
      PushControlHandler(controlHandler);
    }
  }

  public void PushControlHandlers(params BaseControlHandler[] controlHandlers)
  {
    for (var i = 0; i < controlHandlers.Length; i++)
    {
      Logger.Info("Pushing (chained) handler: " + controlHandlers[i].ToString());

      _controlHandlers.Push(controlHandlers[i]);
    }

    TryActivateCurrentControlHandler(_activeControlHandler);
  }

  public void InsertControlHandler(int index, BaseControlHandler controlHandler)
  {
    Logger.Info("Inserting handler: " + controlHandler.ToString() + " at index " + index);

    if (index >= _controlHandlers.Count)
    {
      PushControlHandler(controlHandler);
    }
    else
    {
      _controlHandlers.Insert(index, controlHandler);
    }
  }

  public void InsertControlHandlerBeforeCurrent(BaseControlHandler controlHandler)
  {
    var index = Mathf.Max(0, _controlHandlers.Count - 1);

    Logger.Info("Inserting handler: " + controlHandler.ToString() + " at index " + index);

    _controlHandlers.Insert(index, controlHandler);
  }

  public void PushControlHandler(BaseControlHandler controlHandler)
  {
    Logger.Info("Pushing handler: " + controlHandler.ToString());

    _controlHandlers.Push(controlHandler);

    TryActivateCurrentControlHandler(_activeControlHandler);
  }

  public void RemoveControlHandler(BaseControlHandler controlHandler)
  {
    Logger.Info("Removing handler: " + controlHandler.ToString());

    if (controlHandler == _activeControlHandler)
    {
      var poppedHandler = _controlHandlers.Pop();

      poppedHandler.Dispose();

      TryActivateCurrentControlHandler(poppedHandler);
    }
    else
    {
      _controlHandlers.Remove(controlHandler);

      controlHandler.Dispose();
    }
  }

  public void ExchangeActiveControlHandler(BaseControlHandler controlHandler)
  {
    ExchangeControlHandler(_controlHandlers.Count - 1, controlHandler);
  }

  public void ExchangeControlHandler(int index, BaseControlHandler controlHandler)
  {
    Logger.Info("Exchanging handler " + _controlHandlers[index].ToString() + " (index: " + index + ") with " + controlHandler.ToString());

    if (_controlHandlers[index] == _activeControlHandler)
    {
      var poppedHandler = _controlHandlers.Exchange(index, controlHandler);

      poppedHandler.Dispose();

      TryActivateCurrentControlHandler(poppedHandler);
    }
    else
    {
      _controlHandlers.Exchange(index, controlHandler);
    }
  }
}
