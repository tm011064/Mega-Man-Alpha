using UnityEngine;

public class MovingEnemyController : BaseCharacterController, IPlayerCollidable, ISpawnable
{
  void Awake()
  {
    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();
  }

  public virtual void Reset(Vector3 scale)
  {
  }

  public virtual void OnPlayerCollide(PlayerController playerController)
  {
  }
}
