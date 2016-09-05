using System;
using UnityEngine;

[Serializable]
public class VerticalLockSettings
{
  [Tooltip("If false, all vertical lock settings will be ignored.")]
  public bool Enabled = false;

  [Tooltip("Enables the default vertical lock position. The default position simulates a Super Mario Bros style side scrolling camera which is fixed on the y axis, not reacting to vertical player movement.")]
  public bool EnableDefaultVerticalLockPosition = true;

  [Tooltip("Default is center of the screen.")]
  public float DefaultVerticalLockPosition = 540f;

  [Tooltip("If enabled, the camera follows the player upwards until the \"Top Vertical Lock Position\" is reached.")]
  public bool EnableTopVerticalLock = false;

  [Tooltip("The highest visible location relative to the \"Parent Position Object\" game object space")]
  public float TopVerticalLockPosition = 1080f;

  [Tooltip("If enabled, the camera follows the player downwards until the \"Bottom Vertical Lock Position\" is reached.")]
  public bool EnableBottomVerticalLock = false;

  [Tooltip("The lowest visible location relative to the \"Parent Position Object\" game object space")]
  public float BottomVerticalLockPosition = 0f;

  [HideInInspector]
  public float TopBoundary;

  [HideInInspector]
  public float BottomBoundary;

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

      hash = hash * 23 + BottomBoundary.GetHashCode();
      hash = hash * 23 + BottomVerticalLockPosition.GetHashCode();
      hash = hash * 23 + DefaultVerticalLockPosition.GetHashCode();
      hash = hash * 23 + EnableBottomVerticalLock.GetHashCode();
      hash = hash * 23 + Enabled.GetHashCode();
      hash = hash * 23 + EnableDefaultVerticalLockPosition.GetHashCode();
      hash = hash * 23 + EnableTopVerticalLock.GetHashCode();
      hash = hash * 23 + TopBoundary.GetHashCode();
      hash = hash * 23 + TopVerticalLockPosition.GetHashCode();

      return hash;
    }
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }

  public VerticalLockSettings Clone()
  {
    return MemberwiseClone() as VerticalLockSettings;
  }
}
