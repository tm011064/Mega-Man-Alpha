public class MovingEnemyController : BaseCharacterController, IPlayerCollidable
{
  void Awake()
  {
    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();
  }

  public virtual void Reset(Direction startDirection)
  {
  }

  public virtual void OnPlayerCollide(PlayerController playerController)
  {
  }
}
