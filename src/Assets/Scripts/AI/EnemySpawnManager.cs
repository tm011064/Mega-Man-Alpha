using System.Collections.Generic;
using UnityEngine;

public partial class EnemySpawnManager : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  [SpawnableItemAttribute]
  public GameObject EnemyToSpawnPrefab;

  public RespawnMode RespawnMode = RespawnMode.SpawnOnce;

  public bool DestroySpawnedEnemiesWhenGettingDisabled = false;

  public Vector3 SpawnedPrefabScale = new Vector3(1f, 1f, 1f);

  public float ContinuousSpawnInterval = 10f;

  [Range(1f / 30.0f, float.MaxValue)]
  public float RespawnOnDestroyDelay = .1f;

  private readonly List<GameObject> _spawnedEnemies = new List<GameObject>();

  private ObjectPoolingManager _objectPoolingManager;

  private bool _isDisabling;

  private float _nextSpawnTime;

  private void Spawn()
  {
    var spawnedEnemy = _objectPoolingManager.GetObject(EnemyToSpawnPrefab.name, transform.position);

    var enemyController = spawnedEnemy.GetComponent<ISpawnable>();

    if (enemyController == null)
    {
      throw new MissingComponentException("Enemies spawned by an enemy spawn manager must contain an EnemyController component.");
    }

    enemyController.Reset(SpawnedPrefabScale);

    Logger.Trace("Spawning enemy from {0} at {1}, active: {2}, layer: {3}",
      gameObject.name,
      spawnedEnemy.transform.position,
      spawnedEnemy.activeSelf,
      LayerMask.LayerToName(spawnedEnemy.layer));

    enemyController.GotDisabled += OnEnemyControllerGotDisabled;

    _spawnedEnemies.Add(spawnedEnemy);
  }

  void OnEnemyControllerGotDisabled(BaseMonoBehaviour obj)
  {
    obj.GotDisabled -= OnEnemyControllerGotDisabled; // unsubscribed cause this could belong to a pooled object

    if (!_isDisabling)
    {
      // while we are disabling this object we don't want to touch the spawned enemies list nor respawn
      _spawnedEnemies.Remove(obj.gameObject);

      try
      {
        if (isActiveAndEnabled
          && RespawnMode == RespawnMode.SpawnWhenDestroyed)
        {
          Invoke("Spawn", RespawnOnDestroyDelay);
        }
      }
      catch (MissingReferenceException)
      {
        // we swallow that one, it happens on scene unload when an enemy disables after this object has been finalized
      }
    }
  }

  void Update()
  {
    if (_nextSpawnTime >= 0f
      && Time.time > _nextSpawnTime)
    {
      Spawn();

      if (RespawnMode == RespawnMode.SpawnContinuously
        && ContinuousSpawnInterval > 0f)
      {
        _nextSpawnTime = Time.time + ContinuousSpawnInterval;
      }
      else
      {
        _nextSpawnTime = -1f;
      }
    }
  }

  public void DeactivateSpawnedObjects()
  {
    _isDisabling = true;

    for (var i = _spawnedEnemies.Count - 1; i >= 0; i--)
    {
      _objectPoolingManager.Deactivate(_spawnedEnemies[i]);

      _spawnedEnemies.RemoveAt(i);
    }

    _isDisabling = false;
  }

  void OnDisable()
  {
    Logger.Trace("Disabling EnemySpawnManager {0}", name);

    if (DestroySpawnedEnemiesWhenGettingDisabled)
    {
      _isDisabling = true;

      for (var i = _spawnedEnemies.Count - 1; i >= 0; i--)
      {
        _objectPoolingManager.Deactivate(_spawnedEnemies[i]);

        _spawnedEnemies.RemoveAt(i);
      }

      _isDisabling = false;
    }
  }

  void OnEnable()
  {
    Logger.Trace("Enabling EnemySpawnManager {0}", name);

    _objectPoolingManager = ObjectPoolingManager.Instance;

    _nextSpawnTime = Time.time;
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(EnemyToSpawnPrefab, 1);

    var objectPoolBehaviours = EnemyToSpawnPrefab.GetComponentsInChildren<IObjectPoolBehaviour>(true);

    if (objectPoolBehaviours != null)
    {
      for (var i = 0; i < objectPoolBehaviours.Length; i++)
      {
        foreach (var registrationInfo in objectPoolBehaviours[i].GetObjectPoolRegistrationInfos())
        {
          yield return registrationInfo;
        }
      }
    }
  }
}
