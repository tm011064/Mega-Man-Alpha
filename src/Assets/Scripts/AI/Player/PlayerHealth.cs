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

  public void Reset()
  {
    _currentHealthUnits = _playerController.PlayerHealthSettings.HealthUnits;
  }

  public void KillPlayer()
  {
    _currentHealthUnits = 0;

    _playerController.Respawn();
  }

  public DamageResult ApplyDamage(int healthUnitsToDeduct)
  {
    if ((_playerController.PlayerState & PlayerState.Invincible) != 0)
    {
      return DamageResult.Invincible;
    }

    _currentHealthUnits -= healthUnitsToDeduct;

    if (_currentHealthUnits <= 0)
    {
      KillPlayer();

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
