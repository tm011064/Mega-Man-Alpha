using System;
using UnityEngine;

[Serializable]
public class HorizontalLockSettings
{
  [Tooltip("If false, all horizontal lock settings will be ignored.")]
  public bool Enabled = false;

  [Tooltip("If enabled, the camera follows the player moving right until the \"Right Horizontal Lock Position\" is reached.")]
  public bool EnableRightHorizontalLock = true;

  [Tooltip("The rightmost visible location relative to the \"Parent Position Object\" game object space")]
  public float RightHorizontalLockPosition = 1920f;

  [Tooltip("If enabled, the camera follows the player moving left until the \"Right Horizontal Lock Position\" is reached.")]
  public bool EnableLeftHorizontalLock = true;

  [Tooltip("The leftmost visible location relative to the \"Parent Position Object\" game object space")]
  public float LeftHorizontalLockPosition = 0f;

  [HideInInspector]
  public float RightBoundary;

  [HideInInspector]
  public float LeftBoundary;

  public override bool Equals(object obj)
  {
    return obj != null
      && GetHashCode() == obj.GetHashCode();
  }

  public override int GetHashCode()
  {
    unchecked
    {
      int hash = 17;

      hash = hash * 23 + Enabled.GetHashCode();
      hash = hash * 23 + EnableRightHorizontalLock.GetHashCode();
      hash = hash * 23 + RightHorizontalLockPosition.GetHashCode();
      hash = hash * 23 + EnableLeftHorizontalLock.GetHashCode();
      hash = hash * 23 + LeftHorizontalLockPosition.GetHashCode();
      hash = hash * 23 + RightBoundary.GetHashCode();
      hash = hash * 23 + LeftBoundary.GetHashCode();

      return hash;
    }
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }

  public HorizontalLockSettings Clone()
  {
    return MemberwiseClone() as HorizontalLockSettings;
  }
}
