using System;
using UnityEngine;

[Serializable]
public class SmoothDampMoveSettings
{
  [Tooltip("Camera smooth damping on horizontal character movement.")]
  public float HorizontalSmoothDampTime = .2f;

  [Tooltip("Camera smooth damping on vertical character movement.")]
  public float VerticalSmoothDampTime = .2f;

  [Tooltip("Camera smooth damping on rapid descents. For example, if the player falls down at high speeds, we want the camera to stay tight so the player doesn't move off screen.")]
  public float VerticalRapidDescentSmoothDampTime = .01f;

  [Tooltip("Camera smooth damping on rapid ascents. For example, if the player travel up at high speed due to being catapulted by a trampoline, we want the camera to stay tight so the player doesn't move off screen.")]
  public float VerticalAboveRapidAcsentSmoothDampTime = .2f;

  public override string ToString()
  {
    return string.Format("horizontalSmoothDampTime: {0}; verticalSmoothDampTime: {1}; verticalRapidDescentSmoothDampTime: {2}; verticalAboveRapidAcsentSmoothDampTime: {3};",
      HorizontalSmoothDampTime,
      VerticalSmoothDampTime,
      VerticalRapidDescentSmoothDampTime,
      VerticalAboveRapidAcsentSmoothDampTime);
  }

  public SmoothDampMoveSettings Clone()
  {
    return MemberwiseClone() as SmoothDampMoveSettings;
  }
}
