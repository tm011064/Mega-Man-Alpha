using System;
using UnityEngine;

[Serializable]
public class ProjectileWeaponSettings
{
  public bool EnableAutomaticFire = false;

  public float MaxProjectilesPerSecond = 10f;

  public int MaximumSimultaneouslyActiveProjectiles = 3;

  public float DistancePerSecond = 1000f;

  public GameObject ProjectilePrefab;

  public Vector2 GroundedSpawnLocation;

  public Vector2 AirborneSpawnLocation;

  public Vector2 LadderSpawnLocation;

  public string InputButtonName;

  [Tooltip("The length of the shoot animation to play")]
  public float AnimationClipLength;
}
