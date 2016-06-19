using System;
using UnityEngine;

[Serializable]
public class InputSettings
{
  [Tooltip("This threshold defines at which point an axis value should be ignored. The scale is from 0 to 1, 1 means full press, 0 means no press")]
  public float AxisSensitivityThreshold = .2f;
}
