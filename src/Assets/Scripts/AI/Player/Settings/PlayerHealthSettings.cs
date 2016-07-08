using System;
using UnityEngine;

[Serializable]
public class PlayerHealthSettings
{
  public int HealthUnits = 28;

  public GameObject DeathAnimationPrefab;

  public Vector2 DeathAnimationPrefabOffset = Vector2.zero;
}