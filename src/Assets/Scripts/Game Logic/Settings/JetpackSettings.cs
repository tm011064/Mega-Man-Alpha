using System;
using UnityEngine;

[Serializable]
public class JetpackSettings
{
  [Tooltip("The speed at which the jetpack travels")]
  public float JetpackSpeed = 600f;

  [Tooltip("The speed at which the jetpack can change direction. Higher value means faster change.")]
  public float AirDamping = 2f;

  [Tooltip("If true, the jetpack will float in mid air when not propelled; otherwise, the jetpack will fall down controlled by the 'JetpackSettings -> Float Gravity' force.")]
  public bool AutoFloatWithoutThrust = false;

  [Tooltip("If 'Auto Float Without Thrust' is disabled, this gravity will be used to move the player towards the ground.")]
  public float FloatGravity = -200f;
}
