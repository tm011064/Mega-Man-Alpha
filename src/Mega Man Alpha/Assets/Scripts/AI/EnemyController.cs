public class EnemyController : BaseCharacterController
{
  public Direction StartDirection = Direction.Right;

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
