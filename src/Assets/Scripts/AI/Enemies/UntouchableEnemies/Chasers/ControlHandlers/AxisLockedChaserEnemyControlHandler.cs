using UnityEngine;

public class AxisLockedChaserEnemyControlHandler : EnemyControlHandler<AxisLockedChaserEnemyController>
{
  private PlayerController _playerController;

  private Vector3 _velocity = Vector3.zero;

  private float _skinWidth = .01f;

  private BoxCollider2D _boxCollider2D;

  private float _verticalDistanceBetweenRays;

  private float _horizontalDistanceBetweenRays;

  public AxisLockedChaserEnemyControlHandler(AxisLockedChaserEnemyController groundChaserEnemyController)
    : base(groundChaserEnemyController)
  {
    _playerController = GameManager.Instance.Player;

    _boxCollider2D = _enemyController.GetComponent<BoxCollider2D>();

    Logger.Assert(_boxCollider2D != null, "AxisLockedChaserEnemyControlHandler expects the enemy controller to containa box collider 2d");

    _verticalDistanceBetweenRays = CollisionDetectionUtility.GetVerticalDistanceBetweenRays(_boxCollider2D, _enemyController.transform.localScale, _enemyController.TotalmovementBoundaryCheckRays, _skinWidth);

    _horizontalDistanceBetweenRays = CollisionDetectionUtility.GetHorizontalDistanceBetweenRays(_boxCollider2D, _enemyController.transform.localScale, _enemyController.TotalmovementBoundaryCheckRays, _skinWidth);
  }

  protected override bool DoUpdate()
  {
    var direction = _playerController.transform.position - _enemyController.transform.position;

    var decelerationFactor = 1f;

    if (_enemyController.AxisType == AxisType.Horizontal)
    {
      var horizontalVelocity = 0f;

      if (_playerController.transform.position.y >
            (
              _enemyController.transform.position.y
              + _boxCollider2D.size.y * .5f - _boxCollider2D.offset.y
              + _playerController.BoxColliderSizeDefault.y - _playerController.BoxColliderOffsetDefault.y
            )
          || _playerController.transform.position.y <
            (
              _enemyController.transform.position.y
              - _boxCollider2D.size.y * .5f - _boxCollider2D.offset.y
              - _playerController.BoxColliderSizeDefault.y - _playerController.BoxColliderOffsetDefault.y
            ))
      {
        decelerationFactor = Mathf.Min(Mathf.Abs(direction.x)
          * _enemyController.DecelerationDistanceMultiplicationFactor, 1f);
      }

      if (direction.x > _enemyController.IdleDistanceThreshold) // TODO (Roman): maybe don't hardcode that threshold
      {
        horizontalVelocity = _enemyController.Speed * Time.deltaTime * decelerationFactor;
      }
      else if (direction.x < -_enemyController.IdleDistanceThreshold)
      {
        horizontalVelocity = -_enemyController.Speed * Time.deltaTime * decelerationFactor;
      }

      _velocity.x = Mathf.Lerp(
        _velocity.x,
        horizontalVelocity,
        (decelerationFactor == 1f
            ? _enemyController.SmoothDampFactorWhenDecelerationIsOff
            : _enemyController.SmoothDampFactorWhenDecelerationIsOn
          ) * Time.deltaTime);

      _enemyController.transform.Translate(
        CollisionDetectionUtility.AdjustDeltaMovementWithCollisionCheck(
          AxisType.Horizontal
          , _boxCollider2D
          , _enemyController.MovementBoundaryObjectLayerMask
          , _velocity
          , _skinWidth
          , _enemyController.TotalmovementBoundaryCheckRays
          , _horizontalDistanceBetweenRays));
    }
    else if (_enemyController.AxisType == AxisType.Vertical)
    {
      var verticalVelocity = 0f;

      if (_playerController.transform.position.x >
            (
              _enemyController.transform.position.x
              + _boxCollider2D.size.x * .5f - _boxCollider2D.offset.x
              + _playerController.BoxColliderSizeDefault.x - _playerController.BoxColliderOffsetDefault.x
            )
          || _playerController.transform.position.x <
            (
              _enemyController.transform.position.x
              - _boxCollider2D.size.x * .5f - _boxCollider2D.offset.x
              - _playerController.BoxColliderSizeDefault.x - _playerController.BoxColliderOffsetDefault.x
            ))
      {
        decelerationFactor = Mathf.Min(Mathf.Abs(direction.y) * _enemyController.DecelerationDistanceMultiplicationFactor, 1f);
      }

      if (direction.y > _enemyController.IdleDistanceThreshold) // TODO (Roman): maybe don't hardcode that threshold
      {
        verticalVelocity = _enemyController.Speed * Time.deltaTime * decelerationFactor;
      }
      else if (direction.y < -_enemyController.IdleDistanceThreshold)
      {
        verticalVelocity = -_enemyController.Speed * Time.deltaTime * decelerationFactor;
      }

      _velocity.y = Mathf.Lerp(
        _velocity.y,
        verticalVelocity,
        (decelerationFactor == 1f
            ? _enemyController.SmoothDampFactorWhenDecelerationIsOff
            : _enemyController.SmoothDampFactorWhenDecelerationIsOn)
            * Time.deltaTime);

      _enemyController.transform.Translate(
        CollisionDetectionUtility.AdjustDeltaMovementWithCollisionCheck(
          AxisType.Vertical
          , _boxCollider2D
          , _enemyController.MovementBoundaryObjectLayerMask
          , _velocity
          , _skinWidth
          , _enemyController.TotalmovementBoundaryCheckRays
          , _verticalDistanceBetweenRays));
    }

    return true;
  }
}
