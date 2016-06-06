using UnityEngine;

public struct MoveCalculationResult
{
  public CharacterCollisionState2D CollisionState;

  public Vector3 DeltaMovement;

  public Vector3 OriginalDeltaMovement;

  public bool IsGoingUpSlope;
}
