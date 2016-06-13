using System;

[Serializable]
public class ClimbSettings
{
  public bool EnableLadderClimbing = true;

  public float ClimbUpVelocity = 100f;

  public float ClimbDownVelocity = 100f;
}