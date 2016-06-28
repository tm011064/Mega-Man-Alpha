using System;
using UnityEngine;

[Serializable]
public class ProjectileWeaponSettings
{
  public bool EnableAutomaticFire = false;

  public float AutomaticFireProjectilesPerSecond = 10f;

  public int MaximumSimultaneouslyActiveProjectiles = 3;

  public float MaxSpeed = 1000f;

  public GameObject ProjectilePrefab;

  public Vector2 SpawnLocation;

  public string InputButtonName;
}
