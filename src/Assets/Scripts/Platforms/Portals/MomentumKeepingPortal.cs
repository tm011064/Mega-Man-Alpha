using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class MomentumKeepingPortal : BaseMonoBehaviour
{
  [Tooltip("The other end of the portal where the player gets ejected.")]
  public GameObject ConnectedPortal;

  [Tooltip("When going through portal A and exiting at portal B with emission angle 180 deg, the exit velocity will be the incoming velocity rotated by the emission angle.")]
  public float EmissionAngle;

  [Tooltip("This can be used to multiply the exit .")]
  public Vector2 EmissionVelocityMultiplier = new Vector2(1, 1);

  public Vector3 SpawnOffset = Vector3.zero;

  public Vector2 IncommingVectorVelocityMultiplier = new Vector2(1, 1);

  private MomentumKeepingPortal _connectedMomentumKeepingPortal;

  void Awake()
  {
    _connectedMomentumKeepingPortal = ConnectedPortal.GetComponent<MomentumKeepingPortal>();

    Logger.Assert(_connectedMomentumKeepingPortal != null, "Connected Portal must contain MomentumKeepingPortal script.");
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    var playerController = GameManager.Instance.Player;

    var velocity = playerController.CharacterPhysicsManager.Velocity;

    velocity = new Vector3(
      velocity.x * _connectedMomentumKeepingPortal.IncommingVectorVelocityMultiplier.x,
      velocity.y * _connectedMomentumKeepingPortal.IncommingVectorVelocityMultiplier.y,
      0f);

    var angle = Mathf.Atan2(velocity.y, velocity.x);

    var exitAngle = _connectedMomentumKeepingPortal.EmissionAngle * Mathf.Deg2Rad + angle;

    var exitVelocity = new Vector3(Mathf.Cos(exitAngle), Mathf.Sin(exitAngle), 0f);

    exitVelocity.x *= velocity.magnitude * _connectedMomentumKeepingPortal.EmissionVelocityMultiplier.x;
    exitVelocity.y *= velocity.magnitude * _connectedMomentumKeepingPortal.EmissionVelocityMultiplier.y;

    Debug.Log(name
      + ": Incoming velocity: " + playerController.CharacterPhysicsManager.Velocity
      + ", adjusted velocity: " + velocity
      + ", angle: " + angle * Mathf.Rad2Deg
      + ", exit angle: " + exitAngle * Mathf.Rad2Deg
      + ", exit velocity: " + exitVelocity);

    playerController.transform.position =
      ConnectedPortal.transform.position + _connectedMomentumKeepingPortal.SpawnOffset;

    playerController.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below = false;

    playerController.CharacterPhysicsManager.Velocity = exitVelocity;
  }
}
