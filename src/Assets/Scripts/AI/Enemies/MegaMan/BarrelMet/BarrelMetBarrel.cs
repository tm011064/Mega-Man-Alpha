using UnityEngine;

public class BarrelMetBarrel : MovingEnemyController
{
  public float Speed = 200f;

  public float Gravity = -3960f;

  public int PlayerDamageUnits = 1;

  public ObjectDestructionSettings ObjectDestructionSettings;

  void Awake()
  {
    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();

    CharacterPhysicsManager.BoxCollider = GetComponent<BoxCollider2D>();
    CharacterPhysicsManager.PrimeRaycastOrigins();
    CharacterPhysicsManager.RecalculateDistanceBetweenRays();
  }

  void OnEnable()
  {
    if (ObjectDestructionSettings.DestroyWhenOffScreen)
    {
      StartVisibilityChecks(ObjectDestructionSettings.VisibilityCheckInterval, CharacterPhysicsManager.BoxCollider);
    }
  }

  protected override void OnGotHidden()
  {
    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }

  public override void OnPlayerCollide(PlayerController playerController)
  {
    playerController.PlayerHealth.ApplyDamage(PlayerDamageUnits);
  }

  public void TriggerBarrelThrow(BallisticTrajectorySettings ballisticTrajectorySettings, Direction direction)
  {
    PushControlHandlers(
      new BarrelMetBarrelRollControlHandler(this, direction),
      new BarrelMetBarrelThrowControlHandler(this, direction, ballisticTrajectorySettings));
  }
}
