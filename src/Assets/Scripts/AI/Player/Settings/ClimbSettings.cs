using System;
using UnityEngine;

[Serializable]
public class ClimbSettings
{
  public bool EnableLadderClimbing = true;

  public float ClimbUpVelocity = 100f;

  public float ClimbDownVelocity = 100f;

  [Tooltip("Specifies the minimum time a character has to fall before being able to climb the same ladder again")]
  public float MinFallDuration = .5f;
}