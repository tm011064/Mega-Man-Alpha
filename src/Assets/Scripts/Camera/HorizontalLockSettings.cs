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

  public override string ToString()
  {
    return string.Format("enabled: {0}; enableRightHorizontalLock: {1}; rightHorizontalLockPosition: {2}; rightBoundary: {3}; enableLeftHorizontalLock: {4}; leftHorizontalLockPosition: {5}; leftBoundary: {6};",
      Enabled,
      EnableRightHorizontalLock,
      RightHorizontalLockPosition,
      RightBoundary,
      EnableLeftHorizontalLock,
      LeftHorizontalLockPosition,
      LeftBoundary);
  }

  public HorizontalLockSettings Clone()
  {
    return MemberwiseClone() as HorizontalLockSettings;
  }
}
