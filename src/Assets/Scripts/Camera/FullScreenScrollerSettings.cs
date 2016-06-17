using System;
using UnityEngine;

[Serializable]
public class FullScreenScrollSettings
{
  [Tooltip("Time it takes to scroll from one room to another. During this time the character won't be able to move")]
  public float TransitionTime = 1f;

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
