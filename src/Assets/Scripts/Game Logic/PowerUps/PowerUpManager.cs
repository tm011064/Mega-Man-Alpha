using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class PowerUpManager
{
  private GameManager _gameManager;

  private int _powerMeter;

  private List<PowerUpType> _orderedPowerUpInventory = new List<PowerUpType>();

  private PowerUpType? _currentPowerUpItem = null;

  private BaseControlHandler _currentPowerUpControlHandler = null;

  public PowerUpManager(GameManager gameManager)
  {
    _gameManager = gameManager;
  }

  [HideInInspector]
  public int PowerMeter
  {
    get { return _powerMeter; }
    set { _powerMeter = value; }
  }

  public void ApplyNextInventoryPowerUpItem()
  {
    Logger.Info("Switch to next powerup item");

    if (_orderedPowerUpInventory.Count > 0)
    {
      int index = 0;

      if (_currentPowerUpItem.HasValue)
      {
        index = _orderedPowerUpInventory.IndexOf(_currentPowerUpItem.Value);

        if (index == _orderedPowerUpInventory.Count - 1)
        {
          index = 0;
        }
        else
        {
          index++;
        }
      }

      ApplyPowerUpItem(_orderedPowerUpInventory[index]);
    }
  }

  public void KillPlayer()
  {
    if (_currentPowerUpItem.HasValue)
    {
      _orderedPowerUpInventory.Remove(_currentPowerUpItem.Value);

      _currentPowerUpControlHandler.Dispose();

      _gameManager.Player.RemoveControlHandler(_currentPowerUpControlHandler);

      _currentPowerUpControlHandler = null;

      _currentPowerUpItem = null;

      Logger.Info("Added damage, removed power up item. " + ToString());
    }

    // player died
    Logger.Info("Added damage, respawn. " + ToString());

    // TODO (Roman): this should be somewhere else - this is just test code
    var deathParticles = ObjectPoolingManager.Instance.GetObject(
      GameManager.Instance.GameSettings.PooledObjects.DefaultPlayerDeathParticlePrefab.Prefab.name);

    deathParticles.transform.position = _gameManager.Player.gameObject.transform.position;

    _gameManager.Player.Respawn();
  }
  public DamageResult AddDamage()
  {
    if (_currentPowerUpItem.HasValue)
    {
      _orderedPowerUpInventory.Remove(_currentPowerUpItem.Value);

      _currentPowerUpControlHandler.Dispose();

      _gameManager.Player.RemoveControlHandler(_currentPowerUpControlHandler);

      _currentPowerUpControlHandler = null;

      _currentPowerUpItem = null;

      Logger.Info("Added damage, removed power up item. " + ToString());

      return DamageResult.LostItem;
    }
    else
    {
      if (_powerMeter == 1)
      {
        _powerMeter = 0; // TODO (Roman): notify player for rendering...

        _gameManager.Player.ExchangeControlHandler(0, new BadHealthPlayerControlHandler(_gameManager.Player));

        Logger.Info("Added damage, reduced power meter. " + ToString());

        return DamageResult.LostPower;
      }
      else
      {
        // player died
        Logger.Info("Added damage, respawn. " + ToString());

        // TODO (Roman): this should be somewhere else - this is just test code
        var deathParticles = ObjectPoolingManager.Instance.GetObject(
          GameManager.Instance.GameSettings.PooledObjects.DefaultPlayerDeathParticlePrefab.Prefab.name);

        deathParticles.transform.position = _gameManager.Player.gameObject.transform.position;

        _gameManager.Player.Respawn();

        return DamageResult.IsDead;
      }
    }
  }

  public void ApplyPowerUpItem(PowerUpType powerUpType)
  {
    if (powerUpType == PowerUpType.Basic)
    {
      _powerMeter = 1;

      _gameManager.Player.ExchangeControlHandler(0, new GoodHealthPlayerControlHandler(_gameManager.Player));

      Logger.Info("Added basic power up. " + ToString());
    }
    else
    {
      if (_powerMeter != 1)
      {
        // when getting a non basic power up, we want to set the power meter to 1 so we automatically go 
        // back to that state in case we lose the power up.
        _powerMeter = 1;

        _gameManager.Player.ExchangeControlHandler(0, new GoodHealthPlayerControlHandler(_gameManager.Player));

        Logger.Info("Added basic power up. " + ToString());
      }
      if (!_currentPowerUpItem.HasValue
        || _currentPowerUpItem.Value != powerUpType)
      {
        if (_currentPowerUpItem.HasValue)
        {
          _gameManager.Player.RemoveControlHandler(_currentPowerUpControlHandler);
        }

        switch (powerUpType)
        {
          case PowerUpType.Floater:

            _currentPowerUpControlHandler = new PowerUpFloaterControlHandler(_gameManager.Player, _gameManager.GameSettings.PowerUpSettings);

            break;

          case PowerUpType.DoubleJump:

            _currentPowerUpControlHandler = new PowerUpDoubleJumpControlHandler(_gameManager.Player);

            break;

          case PowerUpType.SpinMeleeAttack:

            _currentPowerUpControlHandler = new PowerUpSpinMeleeAttackControlHandler(_gameManager.Player);

            break;

          case PowerUpType.JetPack:

            _currentPowerUpControlHandler = new PowerUpJetPackControlHandler(_gameManager.Player, 30f, _gameManager.GameSettings.PowerUpSettings);

            break;

          case PowerUpType.Gun:

            _currentPowerUpControlHandler = new PowerUpGunControlHandler(_gameManager.Player, -1f, _gameManager.GameSettings.PowerUpSettings);

            break;
        }

        _gameManager.Player.PushControlHandler(_currentPowerUpControlHandler);

        _currentPowerUpItem = powerUpType;

        if (!_orderedPowerUpInventory.Contains(powerUpType))
        {
          _orderedPowerUpInventory.Add(powerUpType);

          _orderedPowerUpInventory.Sort();
        }

        Logger.Info("Added " + powerUpType.ToString() + " power up. " + ToString());
      }
    }
  }

  public override string ToString()
  {
    var sb = new StringBuilder();

    foreach (PowerUpType powerUpType in _orderedPowerUpInventory)
    {
      sb.Append(powerUpType.ToString() + ", ");
    }

    return "Power Meter: " + _powerMeter
      + ", Current Power-Up: " + (_currentPowerUpItem.HasValue ? _currentPowerUpItem.Value.ToString() : "NULL")
      + ", Inventory: " + (sb.Length == 0 ? "empty" : sb.ToString().TrimEnd(", ".ToCharArray()));
  }

  public enum DamageResult
  {
    IsDead,

    LostPower,

    LostItem
  }
}
