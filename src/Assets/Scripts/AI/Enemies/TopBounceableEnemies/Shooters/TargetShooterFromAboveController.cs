﻿using System.Collections.Generic;
using UnityEngine;

public class TargetShooterFromAboveController : TopBounceableEnemyController, IObjectPoolBehaviour
{
  public float Speed = 200f;

  public float Gravity = -3960f;

  public float ScanAngleClipping = 15f;

  public float ScanRayLength = 2048f;

  public int TotalScanRays = 24;

  public float ShootIntervalDuration = 3f;

  public LayerMask ScanRayCollisionLayers = 0;

  public GameObject ProjectileToSpawn;

  public int MinProjectilesToInstanciate = 10;

  public Vector2 MaxVelocity = new Vector2(512f, 512f);

  [Tooltip("This is the duration the enemy spends at an edge before turning around and moving the other direction. Set to 0 if the player should turn around immediately.")]
  public float EdgeTurnAroundPause = 0f;

  [Tooltip("This is the duration the enemy needs to have continuous sight of the player in order to trigger the detection mechanism. Use -1 if the player should be detected immediately.")]
  public float DetectPlayerDuration = .5f;

  protected override BaseControlHandler ApplyDamageControlHandler
  {
    get { return new DamageTakenPlayerControlHandler(); }
  }

  public override void Reset(Direction startDirection)
  {
    ResetControlHandlers(new TargetShooterFromAboveControlHandler(this, startDirection));
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    return new ObjectPoolRegistrationInfo[]
    {
      new ObjectPoolRegistrationInfo(ProjectileToSpawn, MinProjectilesToInstanciate)
    };
  }
}