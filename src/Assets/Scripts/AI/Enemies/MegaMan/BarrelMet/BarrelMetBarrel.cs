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

  protected override void OnEnable()
  {
    if (ObjectDestructionSettings.DestroyWhenOffScreen)
    {
      // it might happen that the barrel spawn location is not visible when the barrel met is visible. In such a 
      // case, the GotHidden method would not fire as it never was visible in first place. This is why we have to check
      // here whether the barrel is visible
      if (!IsColliderVisible(CharacterPhysicsManager.BoxCollider))
      {
        OnBecameInvisible();

        return;
      }
    }

    base.OnEnable();
  }

  protected override void OnDisable()
  {
    ResetControlHandlers();

    base.OnDisable();
  }

  void OnBecameInvisible()
  {
    if (ObjectDestructionSettings.DestroyWhenOffScreen)
    {
      ObjectPoolingManager.Instance.Deactivate(gameObject);
    }
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
