using System;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
  public int PlayerDamageUnits = 1;

  private GameManager _gameManager;

  private CustomStack<BaseProjectileControlHandler> _controlHandlers = new CustomStack<BaseProjectileControlHandler>();

  private BaseProjectileControlHandler _currentBaseProjectileControlHandler = null;

  public BaseProjectileControlHandler CurrentControlHandler
  {
    get { return _currentBaseProjectileControlHandler; }
  }

  void OnTriggerStay2D(Collider2D col)
  {
    if (col.gameObject == _gameManager.Player.gameObject)
    {
      if ((_gameManager.Player.PlayerState & PlayerState.Invincible) != 0)
      {
        return;
      }

      ObjectPoolingManager.Instance.Deactivate(gameObject);

      _gameManager.Player.PlayerHealth.ApplyDamage(PlayerDamageUnits);
    }
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    // we have to check for player as the hazard might have collided with a hazard destroy trigger
    if (col.gameObject == _gameManager.Player.gameObject)
    {
      if ((_gameManager.Player.PlayerState & PlayerState.Invincible) != 0)
      {
        return;
      }

      ObjectPoolingManager.Instance.Deactivate(gameObject);

      _gameManager.Player.PlayerHealth.ApplyDamage(PlayerDamageUnits);
    }
  }

  private void TryActivateCurrentControlHandler(BaseProjectileControlHandler previousControlHandler)
  {
    _currentBaseProjectileControlHandler = _controlHandlers.Peek();

    while (_currentBaseProjectileControlHandler != null
      && !_currentBaseProjectileControlHandler.TryActivate(previousControlHandler))
    {
      previousControlHandler = _controlHandlers.Pop();

      Logger.Info("Popped handler: " + previousControlHandler.ToString());

      previousControlHandler.Dispose();

      _currentBaseProjectileControlHandler = _controlHandlers.Peek();
    }
  }

  void Update()
  {
    try
    {
      while (!_currentBaseProjectileControlHandler.Update())
      {
        var poppedHandler = _controlHandlers.Pop();

        poppedHandler.Dispose();

        Logger.Info("Popped handler: " + poppedHandler.ToString());

        TryActivateCurrentControlHandler(poppedHandler);
      }
    }
    catch (Exception err)
    {
      Logger.Error("Game object " + name + " misses default control handler.", err);

      throw;
    }
  }

  public void ResetControlHandlers(BaseProjectileControlHandler controlHandler)
  {
    Logger.Info("Resetting character control handlers.");

    for (var i = _controlHandlers.Count - 1; i >= 0; i--)
    {
      Logger.Info("Removing handler: " + _controlHandlers[i].ToString());

      _controlHandlers[i].Dispose();

      _controlHandlers.RemoveAt(i);
    }

    _currentBaseProjectileControlHandler = null;

    PushControlHandler(controlHandler);
  }

  public void PushControlHandler(params BaseProjectileControlHandler[] controlHandlers)
  {
    for (var i = 0; i < controlHandlers.Length; i++)
    {
      Logger.Info("Pushing (chained) handler: " + controlHandlers[i].ToString());

      _controlHandlers.Push(controlHandlers[i]);
    }

    TryActivateCurrentControlHandler(_currentBaseProjectileControlHandler);
  }

  public void InsertControlHandler(int index, BaseProjectileControlHandler controlHandler)
  {
    Logger.Info("Pushing handler: " + controlHandler.ToString());

    if (index >= _controlHandlers.Count)
    {
      PushControlHandler(controlHandler);
    }
    else
    {
      _controlHandlers.Insert(index, controlHandler);
    }
  }

  public void PushControlHandler(BaseProjectileControlHandler controlHandler)
  {
    Logger.Info("Pushing handler: " + controlHandler.ToString());

    _controlHandlers.Push(controlHandler);

    TryActivateCurrentControlHandler(_currentBaseProjectileControlHandler);
  }

  public void RemoveControlHandler(BaseProjectileControlHandler controlHandler)
  {
    Logger.Info("Removing handler: " + controlHandler.ToString());

    if (controlHandler == _currentBaseProjectileControlHandler)
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

  public void ExchangeControlHandler(int index, BaseProjectileControlHandler controlHandler)
  {
    Logger.Info("Exchanging handler " + _controlHandlers[index].ToString() + " (index: " + index + ") with " + controlHandler.ToString());

    if (_controlHandlers[index] == _currentBaseProjectileControlHandler)
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

  void Awake()
  {
    _gameManager = GameManager.Instance;
  }
}
