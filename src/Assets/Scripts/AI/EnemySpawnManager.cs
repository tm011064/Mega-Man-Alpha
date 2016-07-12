using System.Collections.Generic;
using UnityEngine;

public partial class EnemySpawnManager : SpawnBucketItemBehaviour, IObjectPoolBehaviour, ISceneResetable
{
  public RespawnMode RespawnMode = RespawnMode.SpawnOnce;

  public bool DestroySpawnedEnemiesWhenGettingDisabled = false;

  public float ContinuousSpawnInterval = 10f;

  [Range(1f / 30.0f, float.MaxValue)]
  public float RespawnOnDestroyDelay = .1f;

  private readonly List<GameObject> _spawnedEnemies = new List<GameObject>();

  private ObjectPoolingManager _objectPoolingManager;

  private bool _isDisabling;

  private float _nextSpawnTime;

  private GameObject _enemyToSpawnPrefab;

  private ISpawnable _spawnablePrefabComponent;

  private bool _isEnemyToSpawnPrefabLoaded;

  void Awake()
  {
    if (!_isEnemyToSpawnPrefabLoaded)
    {
      LoadEnemyToSpawnPrefab();
    }
  }

  private void LoadEnemyToSpawnPrefab()
  {
    _spawnablePrefabComponent = gameObject.GetComponentInChildren<ISpawnable>(true);

    if (_spawnablePrefabComponent == null)
    {
      throw new MissingReferenceException("Enemy spawn manager '" + name
        + "' does not contain a child object that inmplements an ISpawnable interface");
    }

    _enemyToSpawnPrefab = (_spawnablePrefabComponent as MonoBehaviour).gameObject;

    _enemyToSpawnPrefab.SetActive(false);

    _isEnemyToSpawnPrefabLoaded = true;
  }

  private void Spawn()
  {
    _enemyToSpawnPrefab.transform.position = transform.position;

    if (!_spawnablePrefabComponent.CanSpawn())
    {
      ScheduleSpawn();

      return;
    }

    var spawnedEnemy = _objectPoolingManager.GetObject(_enemyToSpawnPrefab.name, transform.position);

    spawnedEnemy.transform.localScale = _enemyToSpawnPrefab.transform.localScale;

    var spawnable = spawnedEnemy.GetComponent<ISpawnable>();

    spawnable.Reset();

    spawnable.GotDisabled += OnEnemyControllerGotDisabled;

    _spawnedEnemies.Add(spawnedEnemy);
  }

  private void ScheduleSpawn()
  {
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

  void OnEnemyControllerGotDisabled(BaseMonoBehaviour obj)
  {
    obj.GotDisabled -= OnEnemyControllerGotDisabled; // unsubscribed cause this could belong to a pooled object

    if (!_isDisabling)
    {
      // while we are disabling this object we don't want to touch the spawned enemies list nor respawn
      _spawnedEnemies.Remove(obj.gameObject);

      ScheduleSpawn();
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

  public void OnSceneReset()
  {
    if (!DestroySpawnedEnemiesWhenGettingDisabled)
    {
      DeactivateSpawnedObjects();
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

    CancelInvoke("Spawn");
  }

  void OnEnable()
  {
    Logger.Trace("Enabling EnemySpawnManager {0}", name);

    _objectPoolingManager = ObjectPoolingManager.Instance;

    _nextSpawnTime = Time.time;
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    if (!_isEnemyToSpawnPrefabLoaded)
    {
      LoadEnemyToSpawnPrefab();
    }

    return GetObjectPoolRegistrationInfos(_enemyToSpawnPrefab);
  }
}
