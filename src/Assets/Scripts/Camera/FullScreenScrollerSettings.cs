using System;
using UnityEngine;

[Serializable]
public class FullScreenScrollSettings
{
  [Tooltip("Time it takes to scroll from one room to another. During this time the character won't be able to move")]
  public float TransitionTime = 1f;

  public EasingType CameraScrollEasingType = EasingType.Linear;

  [Tooltip("Since the camera scrolls the entire screen, the player also must be pushed in that direction. Otherwise he would disappear")]
  public float PlayerTranslationDistance = 128f;

  public EasingType PlayerTranslationEasingType = EasingType.Linear;

  public float StartScrollFreezeTime = .3f;

  public float EndScrollFreezeTime = .3f;

  public override string ToString()
  {
    return string.Format("transitionTime: {0};",
      TransitionTime);
  }

  public FullScreenScrollSettings Clone()
  {
    return MemberwiseClone() as FullScreenScrollSettings;
  }
}
