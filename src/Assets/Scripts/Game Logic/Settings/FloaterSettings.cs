using System;

[Serializable]
public class FloaterSettings
{
  public float FloaterGravity = -100f;

  public float StartFloatingDuringFallVelocityMultiplier = .1f;

  public float FloaterGravityDecreaseInterpolationFactor = .05f;

  public float FloaterInAirDampingOverride = 3f;
}