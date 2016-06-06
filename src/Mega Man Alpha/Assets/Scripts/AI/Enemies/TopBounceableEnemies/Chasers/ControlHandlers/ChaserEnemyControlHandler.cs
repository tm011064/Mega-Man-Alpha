﻿using UnityEngine;

public class PatrollingChaserEnemyControlHandler : EnemyControlHandler<ChaserEnemyController>
{
  private float _moveDirectionFactor;

  private float _playerInSightDuration = 0f;

  public PatrollingChaserEnemyControlHandler(
    ChaserEnemyController chaserEnemyController,
    Direction startDirection)
    : base(chaserEnemyController, -1f)
  {
    _moveDirectionFactor = startDirection == Direction.Left
      ? -1f
      : 1f;
  }

  protected override bool DoUpdate()
  {
    if (_playerInSightDuration > 0f
      && _playerInSightDuration > _enemyController.detectPlayerDuration)
    {
      _enemyController.PushControlHandler(
        new ChasingChaserEnemyControlHandler(
          _enemyController,
          _enemyController.totalChaseDuration,
          _moveDirectionFactor > 0f
            ? Direction.Right
            : Direction.Left));

      _playerInSightDuration = 0f;

      return true;
    }

    // first move in patrolling mode
    MoveHorizontally(
      ref _moveDirectionFactor,
      _enemyController.speed,
      _enemyController.gravity,
      PlatformEdgeMoveMode.TurnAround,
      _enemyController.edgeTurnAroundPause);

    var startAngleRad = -(_enemyController.scanRayAngle / 2) * Mathf.Deg2Rad;
    var endAngleRad = (_enemyController.scanRayAngle / 2) * Mathf.Deg2Rad;
    var step = _enemyController.scanRayAngle * Mathf.Deg2Rad / (float)(_enemyController.totalScanRays);

    var isSeeingPlayer = false;

    for (var theta = endAngleRad; theta > startAngleRad - step / 2; theta -= step)
    {
      var vector = new Vector2(
        _moveDirectionFactor * (float)(_enemyController.scanRayLength * Mathf.Cos(theta)),
        (float)(_enemyController.scanRayLength * Mathf.Sin(theta)));

      var raycastHit2D = Physics2D.Raycast(
        _enemyController.gameObject.transform.position,
        vector.normalized,
        vector.magnitude,
        _enemyController.scanRayCollisionLayers);

      if (raycastHit2D)
      {
        if (raycastHit2D.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
          _playerInSightDuration += Time.deltaTime;

          isSeeingPlayer = true;

          DrawRay(_enemyController.gameObject.transform.position, raycastHit2D.point.ToVector3() - _enemyController.gameObject.transform.position, Color.red);

          break;
        }
        else
        {
          DrawRay(_enemyController.gameObject.transform.position, raycastHit2D.point.ToVector3() - _enemyController.gameObject.transform.position, Color.grey);
        }
      }
      else
      {
        DrawRay(_enemyController.gameObject.transform.position, vector, Color.grey);
      }
    }

    if (!isSeeingPlayer)
    {
      _playerInSightDuration = 0f;
    }

    return true;
  }
}
