using System;
using UnityEngine;

[Serializable]
public class WallJumpSettings
{
  [Tooltip("If false, wall jumps are disabled.")]
  public bool EnableWallJumps = true;

  [Tooltip("Specifies how long wall jumps are enabled after getting attached to a wall. Set to -1 if the player should always be able to jump off a wall once attached.")]
  public float WallJumpEnabledTime = .5f;

  [Tooltip("Specifies the max downward speed at which a player can jump off a wall when sliding down. If this value is higher than the \"Max Wall Downward Speed\" it won't be used.")]
  public float WallVelocityDownThreshold = -200f;

  [Tooltip("This is the duration of the period where the player can not move the character horizontally after the wall jump has been performed. This can be used to prevent the player from climbing up a wall by moving the player towards the wall after the jump.")]
  public float WallJumpPushOffAxisOverrideTime = .2f;

  [Tooltip("This is the gravity used when the player sticks to a wall and slides down.")]
  public float WallStickGravity = -100f;

  [Tooltip("After the player collided with a wall, the wall jump evaluation time is the duration at which the player can move away from the wall without getting attached. Once attached, the player can't move away except when jumping.")]
  public float WallJumpWallEvaluationDuration = .1f;

  [Tooltip("The downward max speed when sliding down a wall.")]
  public float MaxWallDownwardSpeed = -1000f;

  [Tooltip("This is the minimum distance to the bottom floor a player must have in order to allow walljumps. This prevents unwanted walljumps when a player jumps into a low wall.")]
  public float MinDistanceFromFloor = 128f;
}
