﻿using System;
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

  public override string ToString()
  {
    return string.Format("zoomPercentage: {0}; zoomTime: {1}; zoomEasingType: {2};",
      ZoomPercentage,
      ZoomTime,
      ZoomEasingType);
  }

  public ZoomSettings Clone()
  {
    return MemberwiseClone() as ZoomSettings;
  }
}