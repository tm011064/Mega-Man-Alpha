using UnityEngine;

public class AxisLockedChaserEnemyController : EnemyController
{
  public float speed = 800f;

  public float smoothDampFactorWhenDecelerationIsOn = 3f;

  public float smoothDampFactorWhenDecelerationIsOff = .5f;

  public float idleDistanceThreshold = 5f;

  public float decelerationDistanceMultiplicationFactor = .01f;

  public LayerMask movementBoundaryObjectLayerMask = 0;

  public AxisType axisType = AxisType.Horizontal;

  public int totalmovementBoundaryCheckRays = 3;

  public override void Reset(Direction startDirection)
  {
    ResetControlHandlers(new AxisLockedChaserEnemyControlHandler(this));
  }
}

