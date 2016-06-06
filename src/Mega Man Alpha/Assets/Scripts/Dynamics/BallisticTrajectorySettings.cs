using System;
using UnityEngine;

[Serializable]
public class BallisticTrajectorySettings
{
  public bool IsEnabled = false;

  public float Angle = 2f;

  public float ProjectileGravity = -9.81f;

  public Vector2 EndPosition = Vector2.zero;
}
