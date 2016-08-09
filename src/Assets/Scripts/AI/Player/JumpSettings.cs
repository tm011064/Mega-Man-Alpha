using System;
using UnityEngine;

[Serializable]
public class JumpSettings
{
  [Tooltip("Default gravity")]
  public float Gravity = -3960f;

  [Tooltip("Jump height when standing")]
  public float StandJumpHeight = 380f;

  [Tooltip("Jump height when walking")]
  public float WalkJumpHeight = 380f;

  [Tooltip("Only once the character moves at a speed higher than this value the \"Walk Jump Height\" will be applied.")]
  public float WalkJumpHeightSpeedTrigger = 100f;

  [Tooltip("Jump height when running. Player must have exceeded the \"Run Jump Height Speed Trigger\" velocity in order for this height to be used.")]
  public float RunJumpHeight = 400f;

  [Tooltip("Only once the character moves at a speed higher than this value the \"Run Jump Height\" will be applied.")]
  public float RunJumpHeightSpeedTrigger = 800f;

  [Tooltip("This value defines how fast the character can change direction in mid air. Higher value means faster change.")]
  public float InAirDamping = 2.5f;

  [Tooltip("In order to facilitate jumps while running, this value gives the player some leeway and allows him to jump after falling of a platform. Useful when running at full speed.")]
  public float AllowJumpAfterGroundLostThreashold = .05f;

  [Tooltip("The downward max speed when falling. Normally we don't want the player to accelerate indefinitely as it will make controlling the player very difficult.")]
  public float MaxDownwardSpeed = -1800f;

  [Tooltip("If enabled, the character can jump in the opposite direction against inertia when changing direction on the ground. This helps when doing left-right-left jumps.")]
  public bool EnableBackflipOnDirectionChange = true;

  [Tooltip("The horizontal speed applied to the opposite direction while jumping")]
  public float BackflipOnDirectionChangeSpeed = 200f;

  [Tooltip("Useful in case a character's horizontal speed should be throttled while in air")]
  public float MaxHorizontalSpeed = 100000f;
}
