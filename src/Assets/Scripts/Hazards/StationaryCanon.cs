using System;
using System.Collections.Generic;
using UnityEngine;

public class StationaryCanon : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public GameObject ProjectilePrefab;

  public float ProjectileAcceleration = .05f;

  public float ProjectileTargetVelocity = 600f;

  public Space FireDirectionSpace = Space.World;

  public List<CanonFireDirectionVectors> FireDirectionVectorGroups = new List<CanonFireDirectionVectors>();

  public float RoundsPerMinute = 30f;

  public bool OnlyShootWhenInvisible;

  private float _rateOfFireInterval;

  private float _playerInSightDuration;

  private float _lastRoundFiredTime;

  private ObjectPoolingManager _objectPoolingManager;

  private PlayerController _playerController;

  private int _currentfireDirectionVectorGroupIndex = 0;

  private CameraController _cameraController;

  void Awake()
  {
    _rateOfFireInterval = 60f / RoundsPerMinute;

    _cameraController = Camera.main.GetComponent<CameraController>();

    Logger.Assert(FireDirectionVectorGroups.Count > 0, "Please specify at least one fire direction vector. " + name);
  }

  void OnEnable()
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    _playerController = GameManager.Instance.Player;

    _currentfireDirectionVectorGroupIndex = 0;
  }

  void Update()
  {
    if (!OnlyShootWhenInvisible || _cameraController.IsPointVisible(transform.position))
    {
      if (_lastRoundFiredTime + _rateOfFireInterval <= Time.time)
      {
        for (var i = 0; i < FireDirectionVectorGroups[_currentfireDirectionVectorGroupIndex].vectors.Count; i++)
        {
          var enemyProjectileGameObject = _objectPoolingManager.GetObject(ProjectilePrefab.name, transform.position);

          var enemyProjectile = enemyProjectileGameObject.GetComponent<IEnemyProjectile>();

          Logger.Assert(enemyProjectile != null, "Enemy projectile must not be null");

          Vector2 direction = FireDirectionSpace == Space.World
            ? FireDirectionVectorGroups[_currentfireDirectionVectorGroupIndex].vectors[i]
            : (Vector2)transform.TransformDirection(FireDirectionVectorGroups[_currentfireDirectionVectorGroupIndex].vectors[i]);

          enemyProjectile.StartMove(transform.position, direction, ProjectileAcceleration, ProjectileTargetVelocity);
        }

        _currentfireDirectionVectorGroupIndex = _currentfireDirectionVectorGroupIndex == FireDirectionVectorGroups.Count - 1 ? 0 : _currentfireDirectionVectorGroupIndex + 1;

        _lastRoundFiredTime = Time.time;
      }
    }
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    return new ObjectPoolRegistrationInfo[] { new ObjectPoolRegistrationInfo(ProjectilePrefab, 5) };
  }

  [Serializable]
  public class CanonFireDirectionVectors
  {
    public List<Vector2> vectors = new List<Vector2>();
  }
}
