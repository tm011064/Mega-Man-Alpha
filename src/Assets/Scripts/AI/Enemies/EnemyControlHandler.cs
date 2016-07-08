using UnityEngine;

public class EnemyControlHandler<TEnemyController> : BaseControlHandler
  where TEnemyController : MovingEnemyController
{
  protected TEnemyController _enemyController;

  protected float? _pauseAtEdgeEndTime = null;

  private Animator _animator;

  public EnemyControlHandler(TEnemyController enemyController, float duration = -1f, Animator animator = null)
    : base(enemyController.CharacterPhysicsManager, duration)
  {
    _enemyController = enemyController;

    _animator = animator;
  }

  [System.Diagnostics.Conditional("DEBUG")]
  protected void DrawRay(Vector3 start, Vector3 dir, Color color)
  {
    Debug.DrawRay(start, dir, color);
  }

  protected void MoveHorizontally(
    ref float moveDirectionFactor,
    float speed,
    float gravity,
    PlatformEdgeMoveMode platformEdgeMoveMode,
    float edgeTurnAroundPause = 0f,
    float? jumpVelocityY = null)
  {
    var velocity = CharacterPhysicsManager.Velocity;

    if (_pauseAtEdgeEndTime.HasValue)
    {
      if (_pauseAtEdgeEndTime.Value > Time.time)
      {
        velocity.x = 0f;

        if (jumpVelocityY.HasValue)
        {
          velocity.y = jumpVelocityY.Value;
        }

        velocity.y += gravity * Time.deltaTime;

        CharacterPhysicsManager.Move(velocity * Time.deltaTime);

        return;
      }
      else
      {
        // would go over edge, so change direction
        moveDirectionFactor *= -1;

        _pauseAtEdgeEndTime = null;
      }
    }

    // move with constant speed
    velocity.x = moveDirectionFactor * speed;

    if (jumpVelocityY.HasValue)
    {
      velocity.y = jumpVelocityY.Value;
    }
    else if (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below)
    {
      velocity.y = 0;
    }

    // apply gravity before moving
    velocity.y += gravity * Time.deltaTime;

    var moveCalculationResult = CharacterPhysicsManager.CalculateMove(velocity * Time.deltaTime);

    switch (platformEdgeMoveMode)
    {
      case PlatformEdgeMoveMode.TurnAround:

        var isOnEdge = (moveCalculationResult.CollisionState.WasGroundedLastFrame && moveCalculationResult.CollisionState.Below && !moveCalculationResult.CollisionState.IsFullyGrounded); // we are on edge

        if (
              isOnEdge
              || (moveDirectionFactor < 0f && moveCalculationResult.CollisionState.Left)
              || (moveDirectionFactor > 0f && moveCalculationResult.CollisionState.Right)
           )
        {
          if (isOnEdge && edgeTurnAroundPause > 0f)
          {
            velocity.x = 0f;

            CharacterPhysicsManager.Move(velocity * Time.deltaTime);

            _pauseAtEdgeEndTime = Time.time + edgeTurnAroundPause;
          }
          else
          {
            // would go over edge, so change direction
            moveDirectionFactor *= -1;

            velocity.x = moveDirectionFactor * speed;

            CharacterPhysicsManager.Move(velocity * Time.deltaTime);
          }
        }
        else
        {
          CharacterPhysicsManager.PerformMove(moveCalculationResult);
        }
        break;

      case PlatformEdgeMoveMode.FallOff:

        if (
              (moveDirectionFactor < 0f && moveCalculationResult.CollisionState.Left)
              || (moveDirectionFactor > 0f && moveCalculationResult.CollisionState.Right)
           )
        {
          // would go over edge, so change direction
          moveDirectionFactor *= -1;

          velocity.x = moveDirectionFactor * speed;

          CharacterPhysicsManager.Move(velocity * Time.deltaTime);
        }
        else
        {
          CharacterPhysicsManager.PerformMove(moveCalculationResult);
        }

        break;
    }
  }

  protected void PlayAnimation(int shortNameHash)
  {
    var animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);

    if (animatorStateInfo.shortNameHash == shortNameHash)
    {
      return;
    }

    _animator.Play(shortNameHash);
  }

  protected enum PlatformEdgeMoveMode
  {
    TurnAround,

    FallOff
  }
}
