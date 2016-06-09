using UnityEngine;

public class ChaserEnemyController : TopBounceableEnemyController
{
  public float Speed = 200f;

  public float ChaseSpeed = 600f;

  public float Gravity = -3960f;

  public float ScanRayLength = 256f;

  public float ScanRayAngle = 120f;

  public int TotalScanRays = 12;

  public LayerMask ScanRayCollisionLayers = 0;

  [Tooltip("This is the duration the enemy spends at an edge before turning around and moving the other direction. Set to 0 if the player should turn around immediately.")]
  public float EdgeTurnAroundPause = 0f;

  [Tooltip("This is the duration the enemy needs to have continuous sight of the player in order to trigger the detection mechanism. Use -1 if the player should be detected immediately.")]
  public float DetectPlayerDuration = .5f;

  [Tooltip("This is the duration the enemy chases the player once detected. Set to -1 if the chase should go on forever.")]
  public float TotalChaseDuration = 2f;

  protected override BaseControlHandler ApplyDamageControlHandler
  {
    get { return new DamageTakenPlayerControlHandler(); }
  }

  public override void Reset(Direction startDirection)
  {
    ResetControlHandlers(new PatrollingChaserEnemyControlHandler(this, startDirection));
  }
}

