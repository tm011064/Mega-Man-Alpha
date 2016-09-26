using System.Linq;

public class PlayerStateManager
{
  private PlayerState _playerState;

  public void Set(PlayerState playerState)
  {
    _playerState |= playerState;
  }

  public void Unset(PlayerState playerState)
  {
    _playerState &= ~playerState;
  }
  
  public bool Is(params PlayerState[] states)
  {
    return states.All(state => (_playerState & state) != 0);
  }

  public bool IsNot(params PlayerState[] states)
  {
    return states.All(state => (_playerState & state) == 0);
  }

  public bool IsInvincible()
  {
    return (_playerState & PlayerState.Invincible) != 0;
  }

  public bool IsEnemyContactKnockback()
  {
    return (_playerState & PlayerState.EnemyContactKnockback) != 0;
  }

  public bool IsAttachedToWall()
  {
    return (_playerState & PlayerState.AttachedToWall) != 0;
  }

  public bool IsCrouching()
  {
    return (_playerState & PlayerState.Crouching) != 0;
  }

  public bool IsClimbingLadder()
  {
    return (_playerState & PlayerState.ClimbingLadder) != 0;
  }

  public bool IsClimbingLadderTop()
  {
    return (_playerState & PlayerState.ClimbingLadderTop) != 0;
  }

  public bool IsLocked()
  {
    return (_playerState & PlayerState.Locked) != 0;
  }

  public bool IsSliding()
  {
    return (_playerState & PlayerState.Sliding) != 0;
  }
}

