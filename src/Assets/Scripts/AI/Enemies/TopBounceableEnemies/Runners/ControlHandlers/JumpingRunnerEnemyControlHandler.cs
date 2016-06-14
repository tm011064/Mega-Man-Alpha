using UnityEngine;

public class JumpingRunnerEnemyControlHandler : EnemyControlHandler<JumpingRunnerEnemyController>
{
  private float _moveDirectionFactor;

  private float _nextJumpTime;

  public JumpingRunnerEnemyControlHandler(JumpingRunnerEnemyController controller, Direction startDirection)
    : base(controller, -1f)
  {
    _moveDirectionFactor = startDirection == Direction.Left
      ? -1f
      : 1f;

    CharacterPhysicsManager.ControllerBecameGrounded +=
      _ => _nextJumpTime = Time.time + _enemyController.JumpInterval;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (Time.time >= _nextJumpTime
      && CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      MoveHorizontally(
        ref _moveDirectionFactor,
        _enemyController.Speed,
        _enemyController.Gravity,
        PlatformEdgeMoveMode.FallOff,
        jumpVelocityY: Mathf.Sqrt(2f * -_enemyController.Gravity * _enemyController.JumpHeight));

      _nextJumpTime = Time.time + _enemyController.JumpInterval;
    }
    else
    {
      MoveHorizontally(ref _moveDirectionFactor, _enemyController.Speed, _enemyController.Gravity, PlatformEdgeMoveMode.FallOff);
    }

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}

