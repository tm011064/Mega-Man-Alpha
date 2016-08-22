using System.Collections.Generic;
using UnityEngine;

public partial class BreakablePlatform : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public float StableDuration = .5f;

  public float FallGravity = -3960f;

  [Tooltip("The amount of time it takes until the platfrom reappears once it fell. Set to -1 if respawning should be disabled.")]
  public float RespawnTime = 2f;

  public GameObject PlatformPrefab;

  public PlatformBreakMode BreakMode = PlatformBreakMode.FallDown;

  private ObjectPoolingManager _objectPoolingManager;

  private List<SpawnRoutine> _spawnedObjects = new List<SpawnRoutine>();

  private Vector3 _spawnLocation;

  private float _nextSpawnTime = -1f;

  private void Spawn()
  {
    _spawnedObjects.Add(new SpawnRoutine(_objectPoolingManager, PlatformPrefab, _spawnLocation, StableDuration, FallGravity));
  }

  void Update()
  {
    for (var i = _spawnedObjects.Count - 1; i >= 0; i--)
    {
      var startFalling = _spawnedObjects[i].StartedFalling();

      if (!startFalling)
      {
        _spawnedObjects[i].Update();
      }

      if (RespawnTime >= 0f && startFalling)
      {
        // if we just started to fall, we want to respawn
        if (BreakMode == PlatformBreakMode.Disappear)
        {
          _objectPoolingManager.Deactivate(_spawnedObjects[i].GameObject);

          _spawnedObjects.RemoveAt(i);
        }

        _nextSpawnTime = Time.time + RespawnTime;
      }
    }

    if (_nextSpawnTime > 0f && Time.time >= _nextSpawnTime)
    {
      Spawn();

      _nextSpawnTime = -1f;
    }
  }

  void OnBeforePoolingManagerDeactivated(GameObject obj)
  {
    for (var i = _spawnedObjects.Count - 1; i >= 0; i--)
    {
      if (_spawnedObjects[i].GameObject == obj)
      {
        // object was deactivated, so we remove it from the list. That way it won't be called at the Update() method
        _spawnedObjects.RemoveAt(i);

        break;
      }
    }
  }

  void OnDisable()
  {
    for (var i = _spawnedObjects.Count - 1; i >= 0; i--)
    {
      _objectPoolingManager.Deactivate(_spawnedObjects[i].GameObject);
    }

    _spawnedObjects.Clear();

    _nextSpawnTime = -1f;
  }

  void OnEnable()
  {
    _spawnLocation = transform.position;

    if (_objectPoolingManager == null)
    {
      _objectPoolingManager = ObjectPoolingManager.Instance;

      if (BreakMode == PlatformBreakMode.FallDown)
      {
        // TODO (Roman): do we need to unsubscribe?
        _objectPoolingManager.BeforeDeactivated += OnBeforePoolingManagerDeactivated;
      }
    }

    Spawn();
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(PlatformPrefab);
  }

  public enum PlatformBreakMode
  {
    FallDown,

    Disappear
  }

  private class SpawnRoutine
  {
    public GameObject GameObject;

    private Vector2 _velocity = Vector2.zero;

    private ObjectPoolingManager _objectPoolingManager;

    private SpawnRoutineState _currentState;

    private float _fallStartTime;

    private float _respawnTime;

    private float _stableDuration;

    private float _fallGravity;

    public bool StartedFalling()
    {
      if (_currentState == SpawnRoutineState.PlayerLanded
        && Time.time >= _fallStartTime)
      {
        _currentState = SpawnRoutineState.Falling;

        return true;
      }

      return false;
    }

    public void Update()
    {
      if (_currentState == SpawnRoutineState.Falling)
      {
        var velocity = _velocity;

        velocity.y = velocity.y + _fallGravity * Time.deltaTime;

        GameObject.transform.Translate(velocity * Time.deltaTime, Space.World);

        _velocity = velocity;
      }
    }

    void OnPlayerControllerGotGrounded()
    {
      if (_currentState == SpawnRoutineState.Idle)
      {
        _currentState = SpawnRoutineState.PlayerLanded;

        _fallStartTime = Time.time + _stableDuration;
      }
    }

    public SpawnRoutine(ObjectPoolingManager objectPoolingManager, GameObject platformPrefab, Vector3 spawnLocation, float stableDuration, float fallGravity)
    {
      _fallGravity = fallGravity;

      _stableDuration = stableDuration;

      _currentState = SpawnRoutineState.Idle;

      _objectPoolingManager = objectPoolingManager;

      _velocity = Vector2.zero;

      GameObject = _objectPoolingManager.GetObject(platformPrefab.name);

      GameObject.transform.position = spawnLocation;

      var attachPlayerControllerToObject = GameObject.GetComponentOrThrow<AttachPlayerControllerToObject>();

      attachPlayerControllerToObject.PlayerControllerGotGrounded += OnPlayerControllerGotGrounded;
    }

    private enum SpawnRoutineState
    {
      Idle,

      PlayerLanded,

      Falling
    }
  }
}

