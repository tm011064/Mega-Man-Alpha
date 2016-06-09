using UnityEngine;

public class AxisLockedChaserEnemyController : EnemyController
{
  public float Speed = 800f;

  public float SmoothDampFactorWhenDecelerationIsOn = 3f;

  public float SmoothDampFactorWhenDecelerationIsOff = .5f;

  public float IdleDistanceThreshold = 5f;

  public float DecelerationDistanceMultiplicationFactor = .01f;

  public LayerMask MovementBoundaryObjectLayerMask = 0;

  public AxisType AxisType = AxisType.Horizontal;

  public int TotalmovementBoundaryCheckRays = 3;

  public override void Reset(Direction startDirection)
  {
    ResetControlHandlers(new AxisLockedChaserEnemyControlHandler(this));
  }
}

