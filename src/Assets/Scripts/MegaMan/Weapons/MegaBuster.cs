using System;
using UnityEngine;

public class MegaBuster : MonoBehaviour, IWeapon
{
  public ProjectileWeaponSettings Settings;

  void Awake()
  {
    var didRegistrationSucceed = ObjectPoolingManager.Instance.RegisterPool(
      Settings.ProjectilePrefab,
      Settings.MaximumSimultaneouslyActiveProjectiles,
      Settings.MaximumSimultaneouslyActiveProjectiles);

    if (!didRegistrationSucceed)
    {
      throw new InvalidOperationException("Prefab " + Settings.ProjectilePrefab.name + " could not be registered at pool as it already exists");
    }
  }

  public WeaponControlHandler CreateControlHandler(PlayerController playerController)
  {
    return new MegaBusterControlHandler(playerController, Settings);
  }
}
