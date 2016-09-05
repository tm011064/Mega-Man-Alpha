using System;
using UnityEngine;

[Serializable]
public class ZoomSettings
{
  [Tooltip("Default is 1, 2 means a reduction of 100%, .5 means a magnification of 100%.")]
  public float ZoomPercentage = 1f;

  [Tooltip("The time it takes to zoom to the desired percentage.")]
  public float ZoomTime;

  [Tooltip("The easing type when zooming in or out to the desired zoom percentage.")]
  public EasingType ZoomEasingType;

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

      hash = hash * 23 + ZoomPercentage.GetHashCode();
      hash = hash * 23 + ZoomTime.GetHashCode();
      hash = hash * 23 + ZoomEasingType.GetHashCode();

      return hash;
    }
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }

  public ZoomSettings Clone()
  {
    return MemberwiseClone() as ZoomSettings;
  }
}
