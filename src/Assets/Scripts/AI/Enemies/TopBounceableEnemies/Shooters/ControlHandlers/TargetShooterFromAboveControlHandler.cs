using UnityEngine;

public class TargetShooterFromAboveControlHandler : EnemyControlHandler<TargetShooterFromAboveController>
{
  private ObjectPoolingManager _objectPoolingManager;

  private float _moveDirectionFactor;

  private float _playerInSightDuration = 0f;

  private BoxCollider2D _boxCollider2D;

  private float _lastShotTime;

  public TargetShooterFromAboveControlHandler(TargetShooterFromAboveController targetShooterFromAboveController, Direction startDirection)
    : base(targetShooterFromAboveController, -1f)
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    _moveDirectionFactor = startDirection == Direction.Left
      ? -1f
      : 1f;

    _boxCollider2D = _enemyController.GetComponent<BoxCollider2D>();
  }

  protected override bool DoUpdate()
  {
    if (_playerInSightDuration == 0f // either we don't see the player
      || !_pauseAtEdgeEndTime.HasValue // or we have not reached the edge yet
      )
    {
      MoveHorizontally(
        ref _moveDirectionFactor,
        _enemyController.Speed,
        _enemyController.Gravity,
        PlatformEdgeMoveMode.TurnAround,
        _enemyController.EdgeTurnAroundPause);
    }

    var raycastOrigin = _moveDirectionFactor > 0f
      ? new Vector3(_enemyController.gameObject.transform.position.x + _boxCollider2D.size.x / 2f, _enemyController.gameObject.transform.position.y)
      : new Vector3(_enemyController.gameObject.transform.position.x - _boxCollider2D.size.x / 2f, _enemyController.gameObject.transform.position.y);

    if (_playerInSightDuration > 0f
      && _playerInSightDuration > _enemyController.DetectPlayerDuration
      && _pauseAtEdgeEndTime.HasValue)
    {
      // TODO (Roman): shoot, look at hazard spawn manager and copy/paste logic here
      if (_lastShotTime + _enemyController.ShootIntervalDuration < Time.time)
      {
        var spawnedProjectile = _objectPoolingManager.GetObject(_enemyController.ProjectileToSpawn.name);

        spawnedProjectile.transform.position = raycastOrigin;

        var projectileController = spawnedProjectile.GetComponent<ProjectileController>();

        Logger.Assert(projectileController != null, "A projectile with ballistic trajectory must have a projectile controller script attached.");

        var ballisticTrajectorySettings = new BallisticTrajectorySettings();

        ballisticTrajectorySettings.Angle = 0f; // horizontal launch
        ballisticTrajectorySettings.ProjectileGravity = -200f;

        ballisticTrajectorySettings.EndPosition = new Vector2(
          GameManager.Instance.Player.transform.position.x - raycastOrigin.x
          , Mathf.Min(GameManager.Instance.Player.transform.position.y - raycastOrigin.y, -1f));

        Debug.Log("Endpos: " + ballisticTrajectorySettings.EndPosition + ", " + (GameManager.Instance.Player.transform.position.y - raycastOrigin.y));

        projectileController.PushControlHandler(new BallisticProjectileControlHandler(projectileController, ballisticTrajectorySettings, _enemyController.MaxVelocity));

        _lastShotTime = Time.time;
      }
    }

    var startAngleRad = _moveDirectionFactor > 0f
      ? (-90f + _enemyController.ScanAngleClipping / 2f) * Mathf.Deg2Rad
      : (-180f + _enemyController.ScanAngleClipping / 2f) * Mathf.Deg2Rad;

    var endAngleRad = _moveDirectionFactor > 0f
      ? (0f - _enemyController.ScanAngleClipping / 2f) * Mathf.Deg2Rad
      : (-90f - _enemyController.ScanAngleClipping / 2f) * Mathf.Deg2Rad;

    var step = (endAngleRad - startAngleRad) / (float)(_enemyController.TotalScanRays);

    var isSeeingPlayer = false;

    for (var theta = endAngleRad; theta > startAngleRad - step / 2; theta -= step)
    {
      var vector = new Vector2((float)(_enemyController.ScanRayLength * Mathf.Cos(theta)), (float)(_enemyController.ScanRayLength * Mathf.Sin(theta)));

      var raycastHit2D = Physics2D.Raycast(raycastOrigin, vector.normalized, vector.magnitude, _enemyController.ScanRayCollisionLayers);

      if (raycastHit2D)
      {
        if (raycastHit2D.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
          _playerInSightDuration += Time.deltaTime;

          isSeeingPlayer = true;

          DrawRay(raycastOrigin, raycastHit2D.point.ToVector3() - _enemyController.gameObject.transform.position, Color.red);

          break;
        }
        else
        {
          DrawRay(raycastOrigin, raycastHit2D.point.ToVector3() - _enemyController.gameObject.transform.position, Color.grey);
        }
      }
      else
      {
        DrawRay(raycastOrigin, vector, Color.grey);
      }
    }

    if (!isSeeingPlayer)
    {
      _playerInSightDuration = 0f;
    }
    else if (_pauseAtEdgeEndTime.HasValue)
    {
      _pauseAtEdgeEndTime += Time.deltaTime;
    }

    return true;
  }
}
