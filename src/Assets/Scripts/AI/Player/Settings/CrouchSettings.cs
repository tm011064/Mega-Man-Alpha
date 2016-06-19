using System;
using UnityEngine;

[Serializable]
public class CrouchSettings
{
  public bool EnableCrouching = false;

  public Vector2 BoxColliderOffsetCrouched = Vector2.zero;

  public Vector2 BoxColliderSizeCrouched = Vector2.zero;
}