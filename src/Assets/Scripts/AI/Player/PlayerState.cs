using System;

[Flags]
public enum PlayerState
{
  Invincible = 1,

  EnemyContactKnockback = 2,

  AttachedToWall = 4,

  Crouching = 8,

  ClimbingLadder = 16,

  ClimbingLadderTop = 32,

  Locked = 64,

  Sliding = 128
}
