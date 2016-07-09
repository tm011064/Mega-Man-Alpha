using System;
using UnityEngine;

[Serializable]
public class ObjectDestructionSettings
{
  [Tooltip("If true, the object will be automatically destroyed once it disappears from the visible game area. Use the 'Visiblity Check Interval' to control how often the object's visibility is invalidated")]
  public bool DestroyWhenOffScreen;

  [Tooltip("The visibility check interval in seconds. This field is only used when the 'Destroy When Off Screen' flag is set to true")]
  [Range(.1f, 10f)]
  public float VisibilityCheckInterval = .2f;
}