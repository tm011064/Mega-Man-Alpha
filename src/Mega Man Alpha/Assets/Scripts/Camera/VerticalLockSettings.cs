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

  [HideInInspector]
  public float TranslatedVerticalLockPosition;

  public override string ToString()
  {
    return string.Format(@"enabled: {0}; enableTopVerticalLock: {1}; topVerticalLockPosition: {2}; topBoundary: {3};
enableBottomVerticalLock: {4}; bottomVerticalLockPosition: {5}; bottomBoundary: {6};
enableDefaultVerticalLockPosition: {7}; defaultVerticalLockPosition: {8}; translatedVerticalLockPosition: {9}",
      Enabled,
      EnableTopVerticalLock,
      TopVerticalLockPosition,
      TopBoundary,
      EnableBottomVerticalLock,
      BottomVerticalLockPosition,
      BottomBoundary,
      EnableDefaultVerticalLockPosition,
      DefaultVerticalLockPosition,
      TranslatedVerticalLockPosition);
  }

  public VerticalLockSettings Clone()
  {
    return MemberwiseClone() as VerticalLockSettings;
  }
}
