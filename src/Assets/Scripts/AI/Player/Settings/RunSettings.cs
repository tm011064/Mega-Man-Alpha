using System;
using UnityEngine;

[Serializable]
public class RunSettings
{
  [Tooltip("The normal walk speed")]
  public float WalkSpeed = 600f;

  [Tooltip("If this is enabled, the character can run faster when the player presses the dash button.")]
  public bool EnableRunning = true;

  [Tooltip("The run/dash speed.")]
  public float RunSpeed = 900f;

  [Tooltip("The ground damping used when accelerating. A higher value means slower acceleration.")]
  public float AccelerationGroundDamping = 5f; // how fast do we change direction? higher means faster

  [Tooltip("The ground damping used when decelerating. A higher value means higher deceleration, a lower value means slower deceleration (skidding).")]
  public float DecelerationGroundDamping = 20f; // how fast do we change direction? higher means faster
}
