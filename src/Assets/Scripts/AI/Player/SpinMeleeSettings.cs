﻿using System;
using UnityEngine;

[Serializable]
public class SpinMeleeSettings
{
  public bool EnableSpinMelee = false;

  [HideInInspector]
  [NonSerialized]
  public GameObject SpinMeleeAttackBoxCollider = null;
}