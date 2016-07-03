using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlayerController : BaseCharacterController
{
  private const string TRACE_TAG = "PlayerController";

  public WallJumpSettings WallJumpSettings = new WallJumpSettings();

  public JumpSettings JumpSettings = new JumpSettings();

  public RunSettings RunSettings = new RunSettings();

  public ClimbSettings ClimbSettings = new ClimbSettings();

  public SlideSettings SlideSettings = new SlideSettings();

  public CrouchSettings CrouchSettings = new CrouchSettings();

  public IsTakingDamageSettings IsTakingDamageSettings = new IsTakingDamageSettings();

  public InputSettings InputSettings = new InputSettings();

  public Vector2 BoxColliderOffsetWallAttached = Vector2.zero;

  public Vector2 BoxColliderSizeWallAttached = Vector2.zero;

  [HideInInspector]
  public float AdjustedGravity;

  [HideInInspector]
  public Animator Animator;

  [HideInInspector]
  public GameObject Sprite;

  [HideInInspector]
  public BoxCollider2D BoxCollider;

  [HideInInspector]
  public Vector3 SpawnLocation;

  [HideInInspector]
  public GameObject CurrentPlatform = null;

  [HideInInspector]
  public PlayerState PlayerState;

  [HideInInspector]
  public WeaponControlHandler[] WeaponControlHandlers = new WeaponControlHandler[0];

  private RaycastHit2D _lastControllerColliderHit;

  private Vector3 _velocity;

  private WallJumpControlHandler _reusableWallJumpControlHandler;

  private WallJumpEvaluationControlHandler _reusableWallJumpEvaluationControlHandler;

  private GameManager _gameManager;

  public event Action<GroundedPlatformChangedInfo> GroundedPlatformChanged;

  public event Action JumpedThisFrame;

  void Awake()
  {
    // register with game context so this game object can be accessed everywhere
    _gameManager = GameManager.Instance;

    Logger.Info("Playercontroller awoke and added to game context instance.");

    InitializeBoxCollider();

    InitializeSpriteAndAnimator();

    InitializeCharacterPhysicsManager();

    InitializeWeapons();

    _reusableWallJumpControlHandler = new WallJumpControlHandler(this);

    _reusableWallJumpEvaluationControlHandler = new WallJumpEvaluationControlHandler(this);

    PushControlHandler(new GoodHealthPlayerControlHandler(this));

    _gameManager.PowerUpManager.PowerMeter = 1;

    AdjustedGravity = JumpSettings.Gravity;
  }

  private void InitializeWeapons()
  {
    var childTransform = transform.FindChild("Weapons");

    var weapons = childTransform.GetComponents(typeof(IWeapon)).Cast<IWeapon>();

    var controlHandlers = new List<WeaponControlHandler>();

    foreach (var weapon in weapons)
    {
      controlHandlers.Add(weapon.CreateControlHandler(this));
    }

    WeaponControlHandlers = controlHandlers.ToArray();
  }

  private void InitializeCharacterPhysicsManager()
  {
    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();

    CharacterPhysicsManager.BoxCollider = GetEnvironmentCollider();

    CharacterPhysicsManager.ControllerCollided += OnControllerCollided;

    CharacterPhysicsManager.ControllerLostGround += OnControllerLostGround;
  }

  private void InitializeBoxCollider()
  {
    BoxCollider = GetEnvironmentCollider();
  }

  private void InitializeSpriteAndAnimator()
  {
    var childTransform = transform.FindChild("SpriteAndAnimator");

    Logger.Assert(
      childTransform != null,
      "Player controller is expected to have a SpriteAndAnimator child object. If this is no longer needed, remove this line in code.");

    Sprite = childTransform.gameObject;

    Animator = childTransform.gameObject.GetComponent<Animator>();
  }

  private BoxCollider2D GetEnvironmentCollider()
  {
    var spriteAndAnimator = transform.FindChild("SpriteAndAnimator");

    var environmentCollider = spriteAndAnimator.FindChild("Environment Collider");

    Logger.Assert(
      environmentCollider != null,
      "Player controller is expected to have an Environment Collider child object. If this is no longer needed, remove this line in code.");

    var boxCollider = environmentCollider.GetComponent<BoxCollider2D>();

    if (boxCollider == null)
    {
      throw new MissingComponentException("Environment Collider expects a BoxCollider2D");
    }

    return boxCollider;
  }

  public bool IsFacingRight()
  {
    return Sprite.transform.localScale.x > 0f;
  }

  public bool IsGrounded()
  {
    return CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below == true;
  }

  public bool IsAirborne()
  {
    return CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below == false;
  }

  public void OnFellFromClimb()
  {
    if (ClimbSettings.MinFallDuration <= 0f)
    {
      return;
    }

    ClimbSettings.EnableLadderClimbing = false;

    Invoke("EnableClimbing", ClimbSettings.MinFallDuration);
  }

  public void OnJumpedThisFrame()
  {
    Logger.Info("Ground Jump executed.");

    var handler = JumpedThisFrame;

    if (handler != null)
    {
      handler.Invoke();
    }
  }

  private void SetCurrentPlatform(GameObject gameObject)
  {
    var handler = GroundedPlatformChanged;

    if (handler != null)
    {
      var previousGameObject = CurrentPlatform;

      CurrentPlatform = gameObject;

      GroundedPlatformChanged(new GroundedPlatformChangedInfo(previousGameObject, CurrentPlatform));
    }
    else
    {
      CurrentPlatform = gameObject;
    }
  }

  void OnControllerLostGround()
  {
    if (CurrentPlatform != null)
    {
      SetCurrentPlatform(null);
    }
  }

  void OnControllerCollided(RaycastHit2D hit)
  {
    // bail out on plain old ground hits cause they arent very interesting
    if (hit.normal.y == 1f)
    {
      if (CurrentPlatform != hit.collider.gameObject)
      {
        SetCurrentPlatform(hit.collider.gameObject);
      }

      return;
    }

    // TODO (Roman): these methods should be optimized and put into constant field...
    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Platforms"))
    {
      if (WallJumpSettings.EnableWallJumps
        && !CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below
        && CharacterPhysicsManager.LastMoveCalculationResult.DeltaMovement.y < 0f
        && (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.CharacterWallState & CharacterWallState.OnWall) != 0)
      {
        // wall jumps work like this: if the player makes contact with a wall, we want to keep track how long he moves towards the
        // wall (based on input axis). If a certain threshold is reached, we are "attached to the wall" which will result in a reduced "slide down"
        // gravity. When a player is on a wall, he can not detach by pressing the opposite direction - the only way to detach is to jump.
        if (CurrentControlHandler != _reusableWallJumpControlHandler
          && CurrentControlHandler != _reusableWallJumpEvaluationControlHandler)
        {
          var wallJumpEnabledTime =
            WallJumpSettings.WallJumpEnabledTime >= 0f
              ? WallJumpSettings.WallJumpWallEvaluationDuration + WallJumpSettings.WallJumpEnabledTime
              : -1f;

          // new event, start evaluation
          if (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Left)
          {
            _reusableWallJumpControlHandler.Reset(wallJumpEnabledTime, Direction.Left, WallJumpSettings);

            _reusableWallJumpEvaluationControlHandler.Reset(WallJumpSettings.WallJumpWallEvaluationDuration, Direction.Left, WallJumpSettings);

            PushControlHandler(_reusableWallJumpControlHandler, _reusableWallJumpEvaluationControlHandler);
          }
          else if (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Right)
          {
            _reusableWallJumpControlHandler.Reset(wallJumpEnabledTime, Direction.Right, WallJumpSettings);

            _reusableWallJumpEvaluationControlHandler.Reset(WallJumpSettings.WallJumpWallEvaluationDuration, Direction.Right, WallJumpSettings);

            PushControlHandler(_reusableWallJumpControlHandler, _reusableWallJumpEvaluationControlHandler);
          }
        }
      }
    }
  }

#if UNITY_EDITOR
  void OnDrawGizmos()
  {
    if (CurrentControlHandler != null)
    {
      CurrentControlHandler.DrawGizmos();
    }
  }
#endif

  public void Respawn()
  {
    _gameManager.RefreshScene(SpawnLocation);

    CharacterPhysicsManager.Reset(SpawnLocation);

    AdjustedGravity = JumpSettings.Gravity;

    ResetControlHandlers(new GoodHealthPlayerControlHandler(this));

    _gameManager.PowerUpManager.PowerMeter = 1;

    transform.parent = null; // just in case we were still attached
  }

  private void EnableClimbing()
  {
    ClimbSettings.EnableLadderClimbing = true;
  }
}