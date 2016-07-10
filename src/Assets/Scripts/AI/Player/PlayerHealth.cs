using System;
using System.Linq;

public class PlayerHealth
{
  private readonly PlayerController _playerController;

  private int _currentHealthUnits;

  public PlayerHealth(PlayerController playerController)
  {
    _playerController = playerController;

    Reset();
  }

  public event Action<int> HealthChanged;

  public void Reset()
  {
    SetHealthUnits(_playerController.PlayerHealthSettings.HealthUnits);
  }

  private void SetHealthUnits(int healthUnits)
  {
    _currentHealthUnits = healthUnits;

    var handler = HealthChanged;

    if (handler != null)
    {
      handler(_currentHealthUnits);
    }
  }

  public DamageResult ApplyDamage(int healthUnitsToDeduct)
  {
    if ((_playerController.PlayerState & PlayerState.Invincible) != 0)
    {
      return DamageResult.Invincible;
    }

    SetHealthUnits(_currentHealthUnits - healthUnitsToDeduct);

    if (_currentHealthUnits <= 0)
    {
      _playerController.OnPlayerDied();

      return DamageResult.Destroyed;
    }
    else
    {
      _playerController.PushControlHandlers(
        _playerController.DamageSettings.GetControlHandlers(_playerController).ToArray());
    }

    return DamageResult.HealthReduced;
  }
}
