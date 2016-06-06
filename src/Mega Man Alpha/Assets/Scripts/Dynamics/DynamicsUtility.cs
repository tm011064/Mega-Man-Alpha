using System;
using UnityEngine;

/// <summary>
/// Good sources: Ballistic trajectories -> http://hyperphysics.phy-astr.gsu.edu/hbase/traj.html
/// </summary>
public static class DynamicsUtility
{
  public static Vector3 GetBallisticVelocity(Vector3 targetPosition, Vector3 startPosition, float angle, float gravity)
  {
    if (angle == 0f)
    {
      return GetBallisticVelocityForHorizontalLaunch(targetPosition, startPosition, gravity);
    }

    var targetDirection = targetPosition - startPosition;

    var deltaHeight = targetDirection.y;

    targetDirection.y = 0;  // retain only the horizontal direction

    var horizontalDistance = targetDirection.magnitude;

    var radAngle = angle * Mathf.Deg2Rad;

    targetDirection.y = horizontalDistance * Mathf.Tan(radAngle);

    horizontalDistance += deltaHeight / Mathf.Tan(radAngle); // correct for small height differences

    // calculate the velocity magnitude
    var velocity = Mathf.Sqrt(horizontalDistance * Math.Abs(gravity) / Mathf.Sin(2 * radAngle));

    return velocity * targetDirection.normalized;
  }

  public static Vector3 GetBallisticVelocityForHorizontalLaunch(Vector3 targetPosition, Vector3 startPosition, float gravity)
  {
    var targetDirection = targetPosition - startPosition;

    var vel = targetDirection.x * Mathf.Sqrt(Mathf.Abs(gravity / (2f * targetDirection.y)));

    return new Vector3(vel, 0f, 0f);
  }
}
