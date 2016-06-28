using System;
using UnityEngine;

public class MegaBusterControlHandler : WeaponControlHandler
{
  private readonly ProjectileWeaponSettings _projectileWeaponSettings;

  private readonly ObjectPoolingManager _objectPoolingManager;

  private float _lastBulletTime;

  public MegaBusterControlHandler(PlayerController playerController, ProjectileWeaponSettings projectileWeaponSettings)
    : base(playerController)
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    _projectileWeaponSettings = projectileWeaponSettings;

    if (_projectileWeaponSettings.AutomaticFireProjectilesPerSecond <= 0f)
    {
      throw new ArgumentException("ProjectileWeaponSettings.AutomaticFireBulletsPerSecond value must be greater than 0");
    }

    if (string.IsNullOrEmpty(_projectileWeaponSettings.InputButtonName))
    {
      throw new ArgumentNullException("Input Button Name must be specified at projectile settings for MegaBusterControlHandler");
    }

    if (!GameManager.InputStateManager.DoesButtonNameExist(_projectileWeaponSettings.InputButtonName))
    {
      throw new ArgumentException("Button name " + _projectileWeaponSettings.InputButtonName + " does not exist");
    }
  }

  private bool IsFireButtonPressed()
  {
    return (
      GameManager
        .InputStateManager
        .GetButtonState(_projectileWeaponSettings.InputButtonName)
        .ButtonPressState & ButtonPressState.IsPressed) != 0;
  }

  private bool IsFireButtonUp()
  {
    return (
      GameManager
        .InputStateManager
        .GetButtonState(_projectileWeaponSettings.InputButtonName)
        .ButtonPressState & ButtonPressState.IsUp) != 0;
  }

  private bool IsWithinAutomaticFireRate()
  {
    return _lastBulletTime + (1f / _projectileWeaponSettings.AutomaticFireProjectilesPerSecond) <= Time.time;
  }

  private bool CanFire()
  {
    if (_projectileWeaponSettings.EnableAutomaticFire)
    {
      return IsFireButtonPressed()
        && IsWithinAutomaticFireRate();
    }

    return IsFireButtonUp();
  }

  private Vector2 GetSpawnLocation(Vector2 direction)
  {
    return new Vector2(
      direction.x > 0f
        ? PlayerController.transform.position.x + _projectileWeaponSettings.SpawnLocation.x
        : PlayerController.transform.position.x - _projectileWeaponSettings.SpawnLocation.x
      , PlayerController.transform.position.y + _projectileWeaponSettings.SpawnLocation.y);
  }

  public override void Update()
  {
    if (CanFire())
    {
      var direction = GetDirectionVector();

      var spawnLocation = GetSpawnLocation(direction);

      var projectile = _objectPoolingManager.GetObject(
        _projectileWeaponSettings.ProjectilePrefab.name,
        spawnLocation);

      if (projectile != null)
      {
        var projectileBehaviour = projectile.GetComponent<PlayerProjectileBehaviour>();

        projectileBehaviour.StartMove(
          spawnLocation,
          direction * _projectileWeaponSettings.MaxSpeed);

        _lastBulletTime = Time.time;
      }
    }
  }
}
