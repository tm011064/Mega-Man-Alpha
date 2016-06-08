using System.Collections.Generic;
using UnityEngine;

public class StationarySentryCanon : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public GameObject ProjectilePrefab;

  public float ProjectileAcceleration = .05f;

  public float ProjectileTargetVelocity = 600f;

  public LayerMask ScanRayCollisionLayers = 0;
  
  public float ScanRayStartAngle = 0f;
  
  public float ScanRayEndAngle = 360f;
  
  public float ScanRayLength = 1280;

  public float TimeNeededToDetectPlayer = .1f;
  
  [Range(.1f, 6000f)]
  public float RoundsPerMinute = 30f;

  private float _startAngleRad;
  
  private float _endAngleRad;
  
  private float _rateOfFireInterval;

  private float _playerInSightDuration;
  
  private float _lastRoundFiredTime;

  private ObjectPoolingManager _objectPoolingManager;
  
  private PlayerController _playerController;

  void Awake()
  {
    _startAngleRad = ScanRayStartAngle * Mathf.Deg2Rad;

    _endAngleRad = ScanRayEndAngle * Mathf.Deg2Rad;

    _rateOfFireInterval = 60f / RoundsPerMinute;
  }

  void OnEnable()
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    _playerController = GameManager.Instance.Player;

    Logger.Info("Enabled sentry canon " + GetHashCode());
  }

  void Update()
  {
    var isSeeingPlayer = false;

    var playerVector = _playerController.transform.position - transform.position;

    var angle = Mathf.Atan2(playerVector.y, playerVector.x);

    if (angle < 0f)
    {
      angle += 2 * Mathf.PI;
    }

    if (angle >= _startAngleRad && angle <= _endAngleRad)
    {
      var raycastHit = Physics2D.Raycast(
        gameObject.transform.position, 
        playerVector.normalized, 
        playerVector.magnitude, 
        ScanRayCollisionLayers);
      
      if (raycastHit && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
      {
        isSeeingPlayer = true;

        _playerInSightDuration += Time.deltaTime;
      }
    }

    DrawRay(gameObject.transform.position, playerVector, isSeeingPlayer ? Color.red : Color.gray);

    if (!isSeeingPlayer)
    {
      _playerInSightDuration = 0f;
    }

    if (_playerInSightDuration >= TimeNeededToDetectPlayer)
    {
      if (_lastRoundFiredTime + _rateOfFireInterval <= Time.time)
      {
        var enemyProjectileGameObject = _objectPoolingManager.GetObject(ProjectilePrefab.name, transform.position);
        
        var enemyProjectile = enemyProjectileGameObject.GetComponent<IEnemyProjectile>();
        
        Logger.Assert(enemyProjectile != null, "Enemy projectile must not be null");

        enemyProjectile.StartMove(transform.position, playerVector, ProjectileAcceleration, ProjectileTargetVelocity);

        _lastRoundFiredTime = Time.time;
      }
    }
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    return new ObjectPoolRegistrationInfo[] { new ObjectPoolRegistrationInfo(ProjectilePrefab, 5) };
  }
  
  [System.Diagnostics.Conditional("DEBUG")]
  private void DrawRay(Vector3 start, Vector3 dir, Color color)
  {
    Debug.DrawRay(start, dir, color);
  }
}
