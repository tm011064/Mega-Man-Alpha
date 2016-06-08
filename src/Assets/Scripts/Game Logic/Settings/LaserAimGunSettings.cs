using System;
using UnityEngine;

[Serializable]
public class LaserAimGunSettings
{
  [Tooltip("All layers that the scan rays can collide with. Should include platforms and player.")]
  public LayerMask ScanRayDirectionDownCollisionLayers = 0;

  [Tooltip("All layers that the scan rays can collide with. Should include platforms and player.")]
  public LayerMask ScanRayDirectionUpCollisionLayers = 0;

  [Tooltip("The length of the scan rays emitted from the position of this gameobject.")]
  public float ScanRayLength = 1280;

  public float BulletsPerSecond = 10;

  [Tooltip("If true, bps rate will be real time rather than slowed time. This means that the bps is in unscaled time even though all the movement is in slow motion.")]
  public bool AllowSlowMotionRealTimeBulletsPerSecond = true;

  [Tooltip("Constant speed of the bullet.")]
  public float BulletSpeed = 2000f;

  [Tooltip("The time it takes to 'reload' slow motion time functionality. After the slow motion phase is over, it takes 'Interval Between Aiming' time before it can be used again.")]
  public float IntervalBetweenAiming = 1f;

  public AnimationCurve SlowMotionFactorMultplierCurve = new AnimationCurve(
    new Keyframe(0f, 1f),
    new Keyframe(.2f, .125f),
    new Keyframe(1.2f, .125f),
    new Keyframe(2f, 1f));

  public bool DoAutoAim = true;

  [Tooltip("Angle used for searching a target to auto aim/lock from the current aim direction.")]
  public float AutoAimSearchAngle = 12f;

  [Range(1, 6)]
  public int TotalAutoAimSearchRaysPerSide = 2;

  [Tooltip("If greater than 0, this will force shot direction to use predefined angles. For example, if the value is 90, there will be 4 possible shot angle directions: 0, 90, 180, 270. Set to -1 if not used.")]
  public float AimHelpAngle = 30;
}
