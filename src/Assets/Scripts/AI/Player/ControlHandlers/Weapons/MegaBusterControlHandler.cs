using System;
using UnityEngine;

public class MegaBusterControlHandler : WeaponControlHandler
{
  private readonly ProjectileWeaponSettings _projectileWeaponSettings;

  private readonly ObjectPoolingManager _objectPoolingManager;

  private float _lastBulletTime = float.MinValue;

  private string _lastAnimationName;

  public MegaBusterControlHandler(PlayerController playerController, ProjectileWeaponSettings projectileWeaponSettings)
    : base(playerController)
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    _projectileWeaponSettings = projectileWeaponSettings;

    if (_projectileWeaponSettings.MaxProjectilesPerSecond <= 0f)
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

  private bool IsWithinRateOfFire()
  {
    return _lastBulletTime + (1f / _projectileWeaponSettings.MaxProjectilesPerSecond) <= Time.time;
  }

  private bool IsClimbingLadder(XYAxisState axisState)
  {
    return (PlayerController.PlayerState & PlayerState.ClimbingLadder) != 0
      && !axisState.IsInVerticalSensitivityDeadZone();
  }

  private bool CanFire(XYAxisState axisState)
  {
    return (_projectileWeaponSettings.EnableAutomaticFire
      ? IsFireButtonPressed()
      : IsFireButtonUp())
        && IsWithinRateOfFire()
        && !IsClimbingLadder(axisState);
  }

  private Vector2 GetSpawnLocation(Vector2 direction)
  {
    var spawnLocation = PlayerController.IsGrounded()
      ? _projectileWeaponSettings.GroundedSpawnLocation
      : ((PlayerController.PlayerState & PlayerState.ClimbingLadder) != 0)
        ? _projectileWeaponSettings.LadderSpawnLocation
        : _projectileWeaponSettings.AirborneSpawnLocation;

    return new Vector2(
      direction.x > 0f
        ? PlayerController.transform.position.x + spawnLocation.x
        : PlayerController.transform.position.x - spawnLocation.x
      , PlayerController.transform.position.y + spawnLocation.y);
  }

  public override PlayerStateUpdateResult Update(XYAxisState axisState)
  {
    if (CanFire(axisState))
    {
      var direction = GetDirectionVector(axisState);

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

        _lastAnimationName = GetAnimationName(axisState);

        return PlayerStateUpdateResult.CreateHandled(
          _lastAnimationName,
          1);
      }
    }

    if (!HasShootAnimationFinished())
    {
      return PlayerStateUpdateResult.CreateHandled(_lastAnimationName, 1);
    }

    return PlayerStateUpdateResult.Unhandled;
  }

  private bool HasShootAnimationFinished()
  {
    return _lastBulletTime + _projectileWeaponSettings.AnimationClipLength < Time.time
      || (_lastAnimationName == "Airborne And Shoot" && PlayerController.IsGrounded())
      || (_lastAnimationName == "Climb And Shoot" && ((PlayerController.PlayerState & PlayerState.ClimbingLadder) == 0));
  }

  private string GetAnimationName(XYAxisState axisState)
  {
    if ((PlayerController.PlayerState & PlayerState.ClimbingLadder) != 0)
    {
      return "Climb And Shoot";
    }

    if (PlayerController.IsAirborne())
    {
      return "Airborne And Shoot";
    }

    if (axisState.IsInHorizontalSensitivityDeadZone())
    {
      return "Stand And Shoot";
    }

    return "Run And Shoot";
  }
}
