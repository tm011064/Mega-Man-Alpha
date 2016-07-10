using System.Collections.Generic;
using UnityEngine;

public partial class HazardSpawnManager : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  [SpawnableItemAttribute]
  public GameObject ProjectileToSpawn;

  [Tooltip("Set to -1 if continuous spawning should be disabled.")]
  public float ContinuousSpawnInterval = 10f;

  [Tooltip("Projectiles get pooled internally, so we want to reuse projectiles that have been destroyed. This number is the minimum number of projectiles available at all time.")]
  public int MinProjectilesToInstanciate = 10;

  public BallisticTrajectorySettings BallisticTrajectorySettings = new BallisticTrajectorySettings();

  private ObjectPoolingManager _objectPoolingManager;

  private float _nextSpawnTime;

  private void Spawn()
  {
    var spawnedProjectile = _objectPoolingManager.GetObject(ProjectileToSpawn.name);

    spawnedProjectile.transform.position = transform.position;

    Logger.TraceFormat("Spawning projectile from {0} ({1}) at {2}, active: {3}, layer: {4}",
      GetHashCode(),
      transform.position,
      spawnedProjectile.transform.position,
      spawnedProjectile.activeSelf,
      LayerMask.LayerToName(spawnedProjectile.layer));

    if (BallisticTrajectorySettings.IsEnabled)
    {
      var projectileController = spawnedProjectile.GetComponent<ProjectileController>();

      Logger.Assert(projectileController != null,
        "A projectile with ballistic trajectory must have a projectile controller script attached.");

      projectileController.PushControlHandler(
        new BallisticProjectileControlHandler(projectileController, BallisticTrajectorySettings));
    }
  }

  void OnDisable()
  {
    Logger.TraceFormat("Disabling HazardSpawnManager {0}", GetHashCode());
  }

  void OnEnable()
  {
    Logger.TraceFormat("Enabling HazardSpawnManager {0}", GetHashCode());

    _nextSpawnTime = Time.time + ContinuousSpawnInterval;
  }

  void FixedUpdate()
  {
    // Note: we can not use a coroutine for this because when spawning on the OnEnable method the transform.position 
    // of a pooled object would still point to the last active position when reactivated.
    if (ContinuousSpawnInterval > 0f)
    {
      if (Time.time > _nextSpawnTime)
      {
        Spawn();

        _nextSpawnTime = Time.time + ContinuousSpawnInterval;
      }
    }
  }

  void Awake()
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    return GetObjectPoolRegistrationInfos(ProjectileToSpawn);
  }
}
