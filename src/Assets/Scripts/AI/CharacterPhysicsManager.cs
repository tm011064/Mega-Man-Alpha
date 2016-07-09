using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterPhysicsManager : BasePhysicsManager
{
  private const string TRACE_TAG = "CharacterPhysicsManager";

  private const string SLIDE_TRACE_TAG = "CharacterPhysicsManagerSlide";

  private const float POSITIVE_ZERO_MOVE_FUDGE_FACTOR = .0001f;

  private const float NEGATIVE_ZERO_MOVE_FUDGE_FACTOR = -.0001f;

  private const float K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR = 0.001f;

  /// <summary>
  /// mask with all layers that the player should interact with
  /// </summary>
  public LayerMask PlatformMask = 0;

  /// <summary>
  /// mask with all layers that should act as one-way platforms. Note that one-way platforms should always be EdgeCollider2Ds. This is private because it does not support being
  /// updated anytime outside of the inspector for now.
  /// </summary>
  [SerializeField]
  private LayerMask OneWayPlatformMask = 0;

  /// <summary>
  /// the max slope angle that the CC2D can climb
  /// </summary>
  /// <value>The slope limit.</value>
  [Range(0, 90f)]
  public float SlopeLimit = 30f;

  /// <summary>
  /// the threshold in the change in vertical movement between frames that constitutes jumping
  /// </summary>
  /// <value>The jumping threshold.</value>
  public float JumpingThreshold = 0.07f;

  /// <summary>
  /// curve for multiplying speed based on slope (negative = down slope and positive = up slope)
  /// </summary>
  public AnimationCurve SlopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 1));

  [Range(2, 20)]
  public int TotalHorizontalRays = 8;

  [Range(2, 20)]
  public int TotalVerticalRays = 4;

  [Tooltip("If true, each move call checks whether the character is fully grounded. Fully grounded means he is not standing over an edge.")]
  public bool PerformFullyGroundedChecks = true;

  [Tooltip("If true, each move call checks whether the character is next to a wall. This is useful for wall jumps.")]
  public bool PerformIsOnWallChecks = true;

  [Tooltip("If true, the player will be able to slide up overhanging edges and only bump his head if the collision is outside the 'Edge Slide Up Corner Width' distance.")]
  public bool EnableEdgeSlideUpHelp;

  [Tooltip("The width of the corner (left and right) that will cause the player to slide up the wall of a platform on a vertical top collision.")]
  public float EdgeSlideUpCornerWidth = 16f;

  [HideInInspector]
  [NonSerialized]
  public Transform Transform;

  [HideInInspector]
  [NonSerialized]
  public BoxCollider2D BoxCollider;

  [HideInInspector]
  [NonSerialized]
  public Rigidbody2D RigidBody2D;

  [HideInInspector]
  [NonSerialized]
  public MoveCalculationResult LastMoveCalculationResult = new MoveCalculationResult();

  [HideInInspector]
  [NonSerialized]
  public Vector3 Velocity;

  [HideInInspector]
  public List<RaycastHit2D> LastRaycastHits = new List<RaycastHit2D>();

  [HideInInspector]
  [NonSerialized]
  public AnimationCurve SlopeSpeedMultiplierOverride = null;

  private LayerMask _platformMaskWithoutOneWay = 0;

  /// <summary>
  /// this is used to calculate the downward ray that is cast to check for slopes. We use the somewhat arbitrary value 75 degrees
  /// to calculate the length of the ray that checks for slopes.
  /// </summary>
  private float _slopeLimitTangent = Mathf.Tan(50f * Mathf.Deg2Rad);

  /// <summary>
  /// holder for our raycast origin corners (TR, TL, BR, BL)
  /// </summary>
  private CharacterRaycastOrigins _raycastOrigins;

  /// <summary>
  /// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
  /// horizontally and vertically so that we can send the events after all collision state is set
  /// </summary>
  private List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>(2);

  private float _verticalDistanceBetweenRays;

  private float _horizontalDistanceBetweenRays;

  [SerializeField]
  [Range(0.001f, 10.3f)]
  private float _skinWidth = 0.02f;

  private float _doubleSkinWidth = .04f;

  private TopEdgeCollisionTestContainer[] _topEdgeCollisionTestContainers;

  public event Action<RaycastHit2D> ControllerCollided;

  public event Action<GameObject> ControllerBecameGrounded;

  public event Action ControllerLostGround;

  /// <summary>
  /// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
  /// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
  /// </summary>
  public float SkinWidth
  {
    get { return _skinWidth; }
    set
    {
      _skinWidth = value;
      _doubleSkinWidth = _skinWidth * 2f;
    }
  }

  void Awake()
  {
    // add our one-way platforms to our normal platform mask so that we can land on them from above
    PlatformMask |= OneWayPlatformMask;

    Transform = GetComponent<Transform>();

    RigidBody2D = GetComponent<Rigidbody2D>();

    // here, we trigger our properties that have setters with bodies
    SkinWidth = _skinWidth;

    _platformMaskWithoutOneWay = PlatformMask;

    _platformMaskWithoutOneWay &= ~OneWayPlatformMask;

    _topEdgeCollisionTestContainers = new TopEdgeCollisionTestContainer[TotalVerticalRays];
  }

  public void Reset(Vector3 position)
  {
    LastMoveCalculationResult = new MoveCalculationResult();

    Velocity = Vector3.zero;

    Transform.position = position;

    _raycastHitsThisFrame.Clear();
  }

  public void AddHorizontalForce(float x)
  {
    Velocity.Set(Velocity.x + x, Velocity.y, Velocity.z);
  }

  public void AddVerticalForce(float y)
  {
    Velocity.Set(Velocity.x, Velocity.y + y, Velocity.z);
  }

  public void AddForce(float x, float y)
  {
    Velocity.Set(Velocity.x + x, Velocity.y + y, Velocity.z);
  }

  public bool CanMoveVertically(float verticalRayDistance, bool allowEdgeSlideUp = false)
  {
    verticalRayDistance += _skinWidth;

    if (EnableEdgeSlideUpHelp && allowEdgeSlideUp)
    {
      // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
      var leftOriginX = _raycastOrigins.TopLeft.x;

      var rightOriginX = _raycastOrigins.BottomRight.x;

      var topOriginY = _raycastOrigins.TopLeft.y;

      var leftAdjustmentBoundaryPosition = leftOriginX - _skinWidth + EdgeSlideUpCornerWidth;

      var rightAdjustmentBoundaryPosition = rightOriginX + _skinWidth - EdgeSlideUpCornerWidth;

      var hasHit = false;

      var hasHitOutsideAdjustmentBoundaries = false;

      var hasHitWithinLeftAdjustmentBoundaries = false;

      var hasHitWithinRightAdjustmentBoundaries = false;

      for (var i = 0; i < TotalVerticalRays; i++)
      {
        _topEdgeCollisionTestContainers[i].InsetLeft = leftOriginX + i * _horizontalDistanceBetweenRays;

        _topEdgeCollisionTestContainers[i].IsWithinLeftAdjustmentArea =
          _topEdgeCollisionTestContainers[i].InsetLeft <= leftAdjustmentBoundaryPosition;

        _topEdgeCollisionTestContainers[i].IsWithinRightAdjustmentArea =
          _topEdgeCollisionTestContainers[i].InsetLeft >= rightAdjustmentBoundaryPosition;

        var ray = new Vector2(_topEdgeCollisionTestContainers[i].InsetLeft, topOriginY);

        _topEdgeCollisionTestContainers[i].RaycastHit2D =
          Physics2D.Raycast(ray, Vector2.up, verticalRayDistance, _platformMaskWithoutOneWay);

        if (_topEdgeCollisionTestContainers[i].RaycastHit2D)
        {
          hasHit = true;

          if (!hasHitOutsideAdjustmentBoundaries
            && !_topEdgeCollisionTestContainers[i].IsWithinLeftAdjustmentArea
            && !_topEdgeCollisionTestContainers[i].IsWithinRightAdjustmentArea)
          {
            hasHitOutsideAdjustmentBoundaries = true;
          }

          if (!hasHitWithinLeftAdjustmentBoundaries
            && _topEdgeCollisionTestContainers[i].IsWithinLeftAdjustmentArea)
          {
            hasHitWithinLeftAdjustmentBoundaries = true;
          }

          if (!hasHitWithinRightAdjustmentBoundaries
            && _topEdgeCollisionTestContainers[i].IsWithinRightAdjustmentArea)
          {
            hasHitWithinRightAdjustmentBoundaries = true;
          }

          var deltaMovementY = _topEdgeCollisionTestContainers[i].RaycastHit2D.point.y - ray.y;

          if (!_topEdgeCollisionTestContainers[i].IsWithinLeftAdjustmentArea
            && !_topEdgeCollisionTestContainers[i].IsWithinRightAdjustmentArea
            && Mathf.Abs(deltaMovementY) < _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
          {
            return false;
          }
        }
      }

      if (hasHit)
      {
        if (!hasHitOutsideAdjustmentBoundaries)
        {
          var leftAdjustmentRaycastHit = new RaycastHit2D();

          // first left
          var leftAdjustmentRay = new Vector2(leftAdjustmentBoundaryPosition, topOriginY);

          leftAdjustmentRaycastHit = Physics2D.Raycast(
            leftAdjustmentRay,
            Vector2.up,
            verticalRayDistance,
            _platformMaskWithoutOneWay);

          if (!leftAdjustmentRaycastHit
            && !hasHitWithinRightAdjustmentBoundaries)
          {
            return true;
          }

          // we reached this line, so do right
          var rightAdjustmentRaycastHit = new RaycastHit2D();

          var rightAdjustmentRay = new Vector2(rightAdjustmentBoundaryPosition, topOriginY);

          rightAdjustmentRaycastHit = Physics2D.Raycast(
            rightAdjustmentRay,
            Vector2.up,
            verticalRayDistance,
            _platformMaskWithoutOneWay);

          if (!rightAdjustmentRaycastHit
            && !hasHitWithinLeftAdjustmentBoundaries)
          {
            return true;
          }
        }
      }
    }
    else
    {
      var initialRayOrigin = _raycastOrigins.TopLeft;

      for (var i = 0; i < TotalVerticalRays; i++)
      {
        var ray = new Vector2(initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y);

        DrawRay(ray, Vector2.up * verticalRayDistance, Color.red);

        var raycastHit = Physics2D.Raycast(ray, Vector2.up, verticalRayDistance, _platformMaskWithoutOneWay);

        if (raycastHit)
        {
          // we need to check whether the hit point is on the edge of the collider. If not, the ray 
          // was sent from within the collider which can happen on moving platforms        
          if (raycastHit.distance != 0f)
          {
            Logger.Trace(TRACE_TAG, "Can not jump [{0}, distance: {1}] because of ray: {2}, hit point: {3}, collider bounds: {4}",
              i,
              raycastHit.distance,
              ray,
              raycastHit.point,
              raycastHit.collider.bounds);

            return false;
          }

          // if the distance is 0, we are inside a collider since the raycast origin is inside the player (due to skin width logic). This can
          // happen when a moving platform moves into a non moving player. In such a case we allow the jump to proceed.
          // in case this distance check doesn't work, we can also use "if (!raycastHit.collider.bounds.IsPointOnEdge(raycastHit.point))"
          Logger.Trace(TRACE_TAG, "Jump raycast hit [{0}, distance: {1}] ignored because we are inside the collider. Ray: {2}, hit point: {3}, collider bounds: {4}",
            i,
            raycastHit.distance,
            ray,
            raycastHit.point,
            raycastHit.collider.bounds);
        }
      }
    }

    return true;
  }

  public MoveCalculationResult SlideDown(Direction platformDirection, float downwardVelocity)
  {
    MoveCalculationResult moveCalculationResult = new MoveCalculationResult();

    moveCalculationResult.CollisionState = new CharacterCollisionState2D();
    moveCalculationResult.CollisionState.CharacterWallState = CharacterWallState.NotOnWall;
    moveCalculationResult.CollisionState.WasGroundedLastFrame = LastMoveCalculationResult.CollisionState.Below;
    moveCalculationResult.CollisionState.LastTimeGrounded = LastMoveCalculationResult.CollisionState.LastTimeGrounded;
    moveCalculationResult.DeltaMovement = new Vector3(0f, downwardVelocity, 0f);
    moveCalculationResult.OriginalDeltaMovement = moveCalculationResult.DeltaMovement;
    moveCalculationResult.CollisionState.Reset();

    _raycastHitsThisFrame.Clear();

    PrimeRaycastOrigins();

    var rayDistance = Mathf.Abs(BoxCollider.size.x) + _skinWidth; // we use the width of the character, this is arbitrary but it works

    var rayDirection = platformDirection == Direction.Right ? Vector2.right : -Vector2.right;

    var initialRayOrigin = platformDirection == Direction.Right ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;

    var raycastHit = new RaycastHit2D();

    var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y);

    DrawRay(ray, rayDirection * rayDistance, Color.white);

    Logger.Trace(SLIDE_TRACE_TAG, "SlideDown -> test ray origin: {0}, ray target position: {1}, delta: {2}, delta target position: {3}, platform direction: {4}",
      ray,
      ray + new Vector2(rayDirection.x * rayDistance, rayDirection.y * rayDistance),
      moveCalculationResult.DeltaMovement,
      Transform.position + moveCalculationResult.DeltaMovement,
      platformDirection);

    // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
    // walk up sloped oneWayPlatforms
    raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, _platformMaskWithoutOneWay);

    if (raycastHit)
    {
      var angle = Vector2.Angle(raycastHit.normal, Vector2.up);

      if (angle >= 90f)
      {
        moveCalculationResult.DeltaMovement.x = 0f;

        moveCalculationResult.DeltaMovement.y = downwardVelocity;
      }
      else
      {
        if (platformDirection == Direction.Right)
        {
          moveCalculationResult.DeltaMovement.x =
            downwardVelocity * Mathf.Cos(angle * Mathf.Deg2Rad);

          moveCalculationResult.DeltaMovement.y =
            Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad)) * moveCalculationResult.DeltaMovement.x;
        }
        else
        {
          moveCalculationResult.DeltaMovement.x =
            -downwardVelocity * Mathf.Cos(angle * Mathf.Deg2Rad);

          moveCalculationResult.DeltaMovement.y =
            Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad)) * -moveCalculationResult.DeltaMovement.x;
        }
      }

      Logger.Trace(SLIDE_TRACE_TAG, "SlideDown -> downward velocity: {0}, deltamovement: {1}, angle: {2}",
        downwardVelocity,
        moveCalculationResult.DeltaMovement,
        angle);

      // remember to remove the skinWidth from our deltaMovement
      if (platformDirection == Direction.Right)
      {
        moveCalculationResult.DeltaMovement.x -= .1f;

        moveCalculationResult.CollisionState.Right = true;
      }
      else
      {
        moveCalculationResult.DeltaMovement.x += .1f;

        moveCalculationResult.CollisionState.Left = true;
      }

      Logger.Trace(SLIDE_TRACE_TAG, "SlideDown -> hit; Hit Point: {0}, Target Delta: {1}, Target Position: {2}",
        raycastHit.point,
        moveCalculationResult.DeltaMovement,
        (Transform.position + moveCalculationResult.DeltaMovement));

      _raycastHitsThisFrame.Add(raycastHit);
    }

    if (raycastHit)
    {
      Transform.Translate(moveCalculationResult.DeltaMovement, Space.World);
    }

    // only calculate velocity if we have a non-zero deltaTime
    if (Time.deltaTime > 0)
    {
      Velocity = moveCalculationResult.DeltaMovement / Time.deltaTime;
    }

    LastMoveCalculationResult = moveCalculationResult;

    LastRaycastHits = new List<RaycastHit2D>(_raycastHitsThisFrame);

    return moveCalculationResult;
  }

  public MoveCalculationResult CalculateMove(Vector3 deltaMovement)
  {
    // set small movements to zero
    if (deltaMovement.x <= POSITIVE_ZERO_MOVE_FUDGE_FACTOR && deltaMovement.x >= NEGATIVE_ZERO_MOVE_FUDGE_FACTOR)
    {
      deltaMovement.x = 0f;
    }

    if (deltaMovement.y <= POSITIVE_ZERO_MOVE_FUDGE_FACTOR && deltaMovement.y >= NEGATIVE_ZERO_MOVE_FUDGE_FACTOR)
    {
      deltaMovement.y = 0f;
    }

    Logger.Trace(TRACE_TAG, "Start move calculation method. Current Position: {0}, Delta Movement: {1}, New Position: {2}",
      Transform.position,
      deltaMovement,
      (Transform.position + deltaMovement));

    var moveCalculationResult = new MoveCalculationResult();

    moveCalculationResult.CollisionState = new CharacterCollisionState2D();
    moveCalculationResult.CollisionState.CharacterWallState = CharacterWallState.NotOnWall;
    moveCalculationResult.CollisionState.WasGroundedLastFrame = LastMoveCalculationResult.CollisionState.Below;
    moveCalculationResult.CollisionState.LastTimeGrounded = LastMoveCalculationResult.CollisionState.LastTimeGrounded;
    moveCalculationResult.DeltaMovement = deltaMovement;
    moveCalculationResult.OriginalDeltaMovement = deltaMovement;

    moveCalculationResult.CollisionState.Reset();

    _raycastHitsThisFrame.Clear();

    PrimeRaycastOrigins();

    // first, we check for a slope below us before moving
    // only check slopes if we are going down and grounded
    if (moveCalculationResult.DeltaMovement.y < 0 && moveCalculationResult.CollisionState.WasGroundedLastFrame)
    {
      HandleVerticalSlope(ref moveCalculationResult);

      Logger.Trace(TRACE_TAG, "After handleVerticalSlope. Delta Movement: {0}, New Position: {1}",
        moveCalculationResult.DeltaMovement,
        (Transform.position + moveCalculationResult.DeltaMovement));
    }

    // now we check movement in the horizontal dir
    if (moveCalculationResult.DeltaMovement.x != 0f)
    {
      MoveHorizontally(ref moveCalculationResult);

      Logger.Trace(TRACE_TAG, "After moveHorizontally. Delta Movement: {0}, New Position: {1}",
        moveCalculationResult.DeltaMovement,
        (Transform.position + moveCalculationResult.DeltaMovement));
    }
    else
    {
      moveCalculationResult.CollisionState.CharacterWallState = GetOnWallState();
    }

    // next, check movement in the vertical dir
    if (moveCalculationResult.DeltaMovement.y != 0)
    {
      if (moveCalculationResult.IsGoingUpSlope)
      {
        MoveVerticallyOnSlope(ref moveCalculationResult);
      }
      else
      {
        MoveVertically(ref moveCalculationResult);
      }

      Logger.Trace(TRACE_TAG, "After moveVertically. Delta Movement: {0}, New Position: {1}",
        moveCalculationResult.DeltaMovement,
        (Transform.position + moveCalculationResult.DeltaMovement));
    }

    if (PerformFullyGroundedChecks
      && moveCalculationResult.DeltaMovement.y <= K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
    {
      moveCalculationResult.CollisionState.IsFullyGrounded = IsFullyGrounded(moveCalculationResult);
    }

    return moveCalculationResult;
  }

  public void PerformMove(MoveCalculationResult moveCalculationResult)
  {
    Transform.Translate(moveCalculationResult.DeltaMovement, Space.World);

    // only calculate velocity if we have a non-zero deltaTime
    if (Time.deltaTime > 0)
    {
      var dx = moveCalculationResult.DeltaMovement.x;
      var dy = moveCalculationResult.DeltaMovement.y;

      if (moveCalculationResult.CollisionState.Left && moveCalculationResult.OriginalDeltaMovement.x < moveCalculationResult.DeltaMovement.x)
      {
        dx = 0f;
      }

      if (moveCalculationResult.CollisionState.Right && moveCalculationResult.OriginalDeltaMovement.x > moveCalculationResult.DeltaMovement.x)
      {
        dx = 0f;
      }

      Velocity = new Vector2(dx, dy) / Time.deltaTime;
    }

    // set our becameGrounded state based on the previous and current collision state
    if (!moveCalculationResult.CollisionState.WasGroundedLastFrame
      && moveCalculationResult.CollisionState.Below)
    {
      moveCalculationResult.CollisionState.BecameGroundedThisFrame = true;

      if (ControllerBecameGrounded != null)
      {
        var rayhitGameObjects = new HashSet<GameObject>();

        for (var i = 0; i < _raycastHitsThisFrame.Count; i++)
        {
          if (!rayhitGameObjects.Contains(_raycastHitsThisFrame[i].collider.gameObject))
          {
            ControllerBecameGrounded(_raycastHitsThisFrame[i].collider.gameObject);

            rayhitGameObjects.Add(_raycastHitsThisFrame[i].collider.gameObject);
          }
        }
      }
    }

    if (moveCalculationResult.CollisionState.WasGroundedLastFrame
      && !moveCalculationResult.CollisionState.Below)
    {
      if (ControllerLostGround != null)
      {
        ControllerLostGround();
      }
    }

    // if we are going up a slope we artificially set a y velocity so we need to zero it out here
    if (moveCalculationResult.IsGoingUpSlope)
    {
      Velocity.y = 0;
    }

    if (ControllerCollided != null)
    {
      for (var i = 0; i < _raycastHitsThisFrame.Count; i++)
      {
        ControllerCollided(_raycastHitsThisFrame[i]);
      }
    }

    LastMoveCalculationResult = moveCalculationResult;

    LastRaycastHits = new List<RaycastHit2D>(_raycastHitsThisFrame);

    Logger.Trace(TRACE_TAG, "Collision state: {0}", LastMoveCalculationResult.CollisionState.ToString());
  }

  /// <summary>
  /// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
  /// stop when run into
  /// </summary>
  public void Move(Vector3 deltaMovement)
  {
    RecalculateDistanceBetweenRays();

    var moveCalculationResult = CalculateMove(deltaMovement);

    PerformMove(moveCalculationResult);
  }

  public void WarpToGrounded()
  {
    do
    {
      Move(new Vector3(0, -1f, 0));
    } while (!LastMoveCalculationResult.CollisionState.Below);
  }

  /// <summary>
  /// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
  /// It is also used in the skinWidth setter in case it is changed at runtime.
  /// </summary>
  public void RecalculateDistanceBetweenRays()
  {
    _horizontalDistanceBetweenRays = (BoxCollider.size.x - _doubleSkinWidth) / (TotalVerticalRays - 1);

    _verticalDistanceBetweenRays = (BoxCollider.size.y - _doubleSkinWidth) / (TotalHorizontalRays - 1);
  }

  public bool IsFloorWithinDistance(float rayDistance)
  {
    rayDistance += _skinWidth;

    for (var i = 0; i < TotalVerticalRays; i++)
    {
      var raycastHit = Physics2D.Raycast(
        new Vector2(_raycastOrigins.BottomLeft.x + i * _horizontalDistanceBetweenRays, _raycastOrigins.BottomLeft.y),
        -Vector2.up,
        rayDistance,
        PlatformMask);

      if (raycastHit)
      {
        return true;
      }
    }

    return false;
  }

  private bool IsFullyGrounded(MoveCalculationResult moveCalculationResult)
  {
    var rayDirection = -Vector2.up;

    var rayDistance = Mathf.Abs(moveCalculationResult.DeltaMovement.y) + _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR;

    var bottomLeftRaycastWithDeltaMovement = Physics2D.Raycast(
      new Vector3(_raycastOrigins.BottomLeft.x, _raycastOrigins.BottomLeft.y) + moveCalculationResult.DeltaMovement,
      rayDirection,
      rayDistance,
      PlatformMask);

    var bottomRightRaycastWithDeltaMovement = Physics2D.Raycast(
      new Vector3(_raycastOrigins.BottomRight.x, _raycastOrigins.BottomRight.y) + moveCalculationResult.DeltaMovement,
      rayDirection,
      rayDistance,
      PlatformMask);

    return bottomLeftRaycastWithDeltaMovement && bottomRightRaycastWithDeltaMovement;
  }

  /// <summary>
  /// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
  /// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
  /// </summary>
  public void PrimeRaycastOrigins()
  {
    var modifiedBounds = BoxCollider.bounds;

    modifiedBounds.Expand(-2f * _skinWidth);

    _raycastOrigins.TopLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);

    _raycastOrigins.BottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);

    _raycastOrigins.BottomLeft = modifiedBounds.min;
  }

  private CharacterWallState GetOnWallState()
  {
    CharacterWallState characterWallState = CharacterWallState.NotOnWall;

    var rayDistance = _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR;

    // check left
    if (Physics2D.Raycast(new Vector2(_raycastOrigins.BottomLeft.x, _raycastOrigins.BottomLeft.y), -Vector2.right, rayDistance, _platformMaskWithoutOneWay)
        && Physics2D.Raycast(new Vector2(_raycastOrigins.BottomLeft.x, _raycastOrigins.TopLeft.y), -Vector2.right, rayDistance, _platformMaskWithoutOneWay)
      )
    {
      characterWallState &= ~CharacterWallState.NotOnWall;
      characterWallState |= CharacterWallState.OnLeftWall;
    }

    // check right
    if (Physics2D.Raycast(new Vector2(_raycastOrigins.BottomRight.x, _raycastOrigins.BottomRight.y), Vector2.right, rayDistance, _platformMaskWithoutOneWay)
        && Physics2D.Raycast(new Vector2(_raycastOrigins.BottomRight.x, _raycastOrigins.TopLeft.y), Vector2.right, rayDistance, _platformMaskWithoutOneWay)
      )
    {
      characterWallState &= ~CharacterWallState.NotOnWall;
      characterWallState |= CharacterWallState.OnRightWall;
    }

    return characterWallState;
  }

  /// <summary>
  /// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
  /// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
  /// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
  /// actually moving the player
  /// </summary>
  private void MoveHorizontally(ref MoveCalculationResult moveCalculationResult)
  {
    var isGoingRight = moveCalculationResult.DeltaMovement.x > 0;

    var rayDistance = Mathf.Abs(moveCalculationResult.DeltaMovement.x) + _skinWidth;

    var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;

    var initialRayOrigin = isGoingRight ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;

    RaycastHit2D raycastHit;
    for (var i = 0; i < TotalHorizontalRays; i++)
    {
      var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays);

      DrawRay(ray, rayDirection * rayDistance, Color.red);

      Logger.Trace(TRACE_TAG, "moveHorizontally -> test ray origin: {0}, ray target position: {1}, delta: {2}, delta target position: {3}",
        ray,
        new Vector2(rayDirection.x * rayDistance, rayDirection.y * rayDistance),
        moveCalculationResult.DeltaMovement,
        Transform.position + moveCalculationResult.DeltaMovement);

      // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
      // walk up sloped oneWayPlatforms
      if (i == 0 && moveCalculationResult.CollisionState.WasGroundedLastFrame)
      {
        raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, PlatformMask);
      }
      else
      {
        raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, _platformMaskWithoutOneWay);
      }

      if (raycastHit)
      {
        if (i == 0)
        {
          // the bottom ray can hit slopes but no other ray can so we have special handling for those cases
          // Note (Roman): I'm passing in the current raycast hit point as reference point for the slope raycasts
          if (HandleHorizontalSlope(ref moveCalculationResult, Vector2.Angle(raycastHit.normal, Vector2.up), raycastHit.point))
          {
            _raycastHitsThisFrame.Add(raycastHit);

            Logger.Trace(TRACE_TAG, "moveHorizontally -> We are on horizontal slope.");

            break;
          }
        }

        moveCalculationResult.DeltaMovement.x = raycastHit.point.x - ray.x;

        rayDistance = Mathf.Abs(moveCalculationResult.DeltaMovement.x);

        // remember to remove the skinWidth from our deltaMovement
        if (isGoingRight)
        {
          moveCalculationResult.DeltaMovement.x -= _skinWidth;
          moveCalculationResult.CollisionState.Right = true;
        }
        else
        {
          moveCalculationResult.DeltaMovement.x += _skinWidth;
          moveCalculationResult.CollisionState.Left = true;
        }

        Logger.Trace(TRACE_TAG, "moveHorizontally -> hit; Hit Point: {0}, Target Delta: {1}, Target Position: {2}",
          raycastHit.point,
          moveCalculationResult.DeltaMovement,
          (Transform.position + moveCalculationResult.DeltaMovement));

        _raycastHitsThisFrame.Add(raycastHit);

        // we add a small fudge factor for the float operations here. if our rayDistance is smaller
        // than the width + fudge bail out because we have a direct impact
        if (rayDistance < _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
        {
          if (i == 0 && Mathf.RoundToInt(Vector2.Angle(raycastHit.normal, Vector2.up)) == 90)
          {
            // if the first ray was a direct hit, we also check the last ray to find out whether the character touches
            // a wall...
            ray = new Vector2(
              initialRayOrigin.x,
              initialRayOrigin.y + (TotalHorizontalRays - 1) * _verticalDistanceBetweenRays);

            Logger.Trace(TRACE_TAG, "moveHorizontally -> first ray hit wall, check whether second ray hits too: ray {0}, ray target position: {1}, delta: {2}, delta target position: {3}",
              ray,
              new Vector2(rayDirection.x * rayDistance, rayDirection.y * rayDistance),
              moveCalculationResult.DeltaMovement,
              Transform.position + moveCalculationResult.DeltaMovement);

            if (i == 0 && moveCalculationResult.CollisionState.WasGroundedLastFrame)
            {
              raycastHit = Physics2D.Raycast(ray, rayDirection, _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR, PlatformMask);
            }
            else
            {
              raycastHit = Physics2D.Raycast(ray, rayDirection, _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR, _platformMaskWithoutOneWay);
            }

            if (raycastHit && Mathf.RoundToInt(Vector2.Angle(raycastHit.normal, Vector2.up)) == 90)
            {
              moveCalculationResult.CollisionState.CharacterWallState &= ~CharacterWallState.NotOnWall;
              moveCalculationResult.CollisionState.CharacterWallState |= (isGoingRight ? CharacterWallState.OnRightWall : CharacterWallState.OnLeftWall);
            }
          }

          break;
        }
      }
    }
  }

  /// <summary>
  /// handles adjusting deltaMovement if we are going up a slope
  /// </summary>
  /// <returns><c>true</c>, if horizontal slope was handled, <c>false</c> otherwise.</returns>
  private bool HandleHorizontalSlope(
    ref MoveCalculationResult moveCalculationResult,
    float angle,
    Vector2 horizontalRaycastHit)
  {
    // disregard 90 degree angles (= walls)
    if (Mathf.RoundToInt(angle) == 90)
    {
      return false;
    }

    // if we can walk on slopes and our angle is small enough we need to move up
    if (angle < SlopeLimit)
    {
      // we only need to adjust the deltaMovement if we are not jumping
      if (moveCalculationResult.DeltaMovement.y < JumpingThreshold)
      {
        // apply the slopeModifier to slow our movement up the slope
        var slopeModifier =
          (SlopeSpeedMultiplierOverride == null
            ? SlopeSpeedMultiplier
            : SlopeSpeedMultiplierOverride)
          .Evaluate(angle);

        var rayOrigin = moveCalculationResult.DeltaMovement.x > 0
          ? new Vector2(horizontalRaycastHit.x - .1f, horizontalRaycastHit.y)
          : new Vector2(horizontalRaycastHit.x + .1f, horizontalRaycastHit.y);

        var currentdelta = Vector2.zero;

        var targetDelta = new Vector2();

        // smooth y movement when we climb. we make the y movement equivalent to the actual y location that corresponds
        // to our new x location using our good friend Pythagoras
        var targetMoveX = moveCalculationResult.DeltaMovement.x * slopeModifier;

        var targetMoveMultiplier = targetMoveX >= 0 ? -1f : 1f;

        targetDelta.x = targetMoveX;
        targetDelta.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * targetDelta.x);

        RaycastHit2D raycastHit;

        do
        {
          // check whether we go through a wall, if so adjust...

          Logger.Trace(TRACE_TAG, "handleHorizontalSlope -> Raycast test; Current Position: {0}, Target Delta: {1}, Target Position: {2}, Current Delta: {3}, Target Move X: {4}, angle: {5}",
            rayOrigin,
            targetDelta,
            (rayOrigin + targetDelta),
            currentdelta,
            targetMoveX,
            angle);

          if (moveCalculationResult.CollisionState.WasGroundedLastFrame)
          {
            raycastHit = Physics2D.Raycast(rayOrigin, targetDelta.normalized, targetDelta.magnitude, PlatformMask);
          }
          else
          {
            raycastHit = Physics2D.Raycast(rayOrigin, targetDelta.normalized, targetDelta.magnitude, _platformMaskWithoutOneWay);
          }

          if (raycastHit)
          {//we crossed an edge when using Pythagoras calculation, so we set the actual delta movement to the ray hit location
            var raycastHitVector = (raycastHit.point - rayOrigin);

            currentdelta += raycastHitVector;

            targetMoveX = targetMoveX + Mathf.Abs(currentdelta.x) * targetMoveMultiplier;

            Logger.Trace(TRACE_TAG, "handleHorizontalSlope -> hit; Hit Point: {5}, Current Position: {0}, Target Delta: {1}, Target Position: {2}, Current Delta: {3}, Target Move X: {4}",
              rayOrigin,
              targetDelta,
              (rayOrigin + targetDelta),
              currentdelta,
              targetMoveX,
              raycastHit.point);

            // we have adjusted the delta, now do the same thing again...
            angle = Vector2.Angle(raycastHit.normal, Vector2.up);

            if (angle < SlopeLimit)
            {
              rayOrigin = rayOrigin + currentdelta;

              targetDelta.x = targetMoveX;
              targetDelta.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * targetDelta.x);
            }
            else
            {
              Logger.Trace(TRACE_TAG, "handleHorizontalSlope -> slope limit exceeded after hit.");

              break;
            }
          }
          else
          {
            currentdelta += targetDelta;

            Logger.Trace(TRACE_TAG, "handleHorizontalSlope -> no hit; final delta movement: {0}, final new position: {1}",
              currentdelta,
              Transform.position + new Vector3(currentdelta.x, currentdelta.y));
          }
        } while (raycastHit);

        moveCalculationResult.DeltaMovement.y = currentdelta.y;

        moveCalculationResult.DeltaMovement.x = currentdelta.x;

        moveCalculationResult.IsGoingUpSlope = true;

        moveCalculationResult.CollisionState.Below = true;

        moveCalculationResult.CollisionState.LastTimeGrounded = Time.time;
      }
      else
      {
        Logger.Trace(TRACE_TAG, "handleHorizontalSlope -> Jump threshold exceeded: deltaMovement.y >= slopeLimit [{0} >= {1}]",
          moveCalculationResult.DeltaMovement.y,
          JumpingThreshold);
      }
    }
    else // too steep. get out of here
    {
      Logger.Trace(TRACE_TAG, "handleHorizontalSlope -> slope limit exceeded.");

      moveCalculationResult.DeltaMovement.x = 0;
    }

    return true;
  }

  private void MoveVerticallyOnSlope(ref MoveCalculationResult moveCalculationResult)
  {
    Logger.Trace(TRACE_TAG, "moveVerticallyOnSlope -> start vert move check");

    var rayDistance = moveCalculationResult.DeltaMovement.magnitude + _skinWidth;

    var rayDirection = moveCalculationResult.DeltaMovement.normalized;

    var initialRayOrigin = _raycastOrigins.TopLeft;

    // if we are moving up, we should ignore the layers in oneWayPlatformMask
    var mask = moveCalculationResult.CollisionState.WasGroundedLastFrame ? PlatformMask : _platformMaskWithoutOneWay;

    RaycastHit2D raycastHit;

    for (var i = 0; i < TotalVerticalRays; i++)
    {
      var ray = new Vector2(initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y);

      DrawRay(ray, rayDirection * rayDistance, Color.red);

      raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);

      Logger.Trace(TRACE_TAG, "moveVerticallyOnSlope -> test ray origin: {0}, ray target position: {1}, delta: {2}, delta target position: {3}",
        ray,
        new Vector2(rayDirection.x * rayDistance, rayDirection.y * rayDistance),
        moveCalculationResult.DeltaMovement,
        Transform.position + moveCalculationResult.DeltaMovement);

      if (raycastHit)
      {
        // set our new deltaMovement and recalculate the rayDistance taking it into account
        moveCalculationResult.DeltaMovement = raycastHit.point - ray;

        Logger.Trace(TRACE_TAG, "moveVerticallyOnSlope -> ray hit; hit point: {0}, new delta: {1}, new delta target position: {2}",
          raycastHit.point,
          moveCalculationResult.DeltaMovement,
          Transform.position + moveCalculationResult.DeltaMovement);

        rayDistance = moveCalculationResult.DeltaMovement.magnitude;

        if (moveCalculationResult.DeltaMovement.x > 0)
        {
          moveCalculationResult.DeltaMovement -=
            new Vector3(rayDirection.x * _skinWidth, rayDirection.y * _skinWidth, 0f);
        }
        else
        {
          moveCalculationResult.DeltaMovement +=
            new Vector3(rayDirection.x * _skinWidth, rayDirection.y * _skinWidth, 0f);
        }

        moveCalculationResult.CollisionState.Above = true;

        _raycastHitsThisFrame.Add(raycastHit);

        // we add a small fudge factor for the float operations here. if our rayDistance is smaller
        // than the width + fudge bail out because we have a direct impact
        if (rayDistance < _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
        {
          break;
        }
      }
    }
  }

  private void MoveVertically(ref MoveCalculationResult moveCalculationResult)
  {
    Logger.Trace(TRACE_TAG, "moveVertically -> start vert move check");

    var isGoingUp = moveCalculationResult.DeltaMovement.y > 0;

    var rayDistance = Mathf.Abs(moveCalculationResult.DeltaMovement.y) + _skinWidth;

    var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;

    // if we are moving up, we should ignore the layers in oneWayPlatformMask
    var mask = isGoingUp ? _platformMaskWithoutOneWay : PlatformMask;

    if (isGoingUp && EnableEdgeSlideUpHelp)
    {
      Logger.Trace(TRACE_TAG, "moveVertically -> going up and top edge collision adjustment enabled");

      // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
      var leftOriginX = _raycastOrigins.TopLeft.x + moveCalculationResult.DeltaMovement.x;

      var rightOriginX = _raycastOrigins.BottomRight.x + moveCalculationResult.DeltaMovement.x;

      var topOriginY = _raycastOrigins.TopLeft.y;

      var leftAdjustmentBoundaryPosition = leftOriginX - _skinWidth + EdgeSlideUpCornerWidth;

      var rightAdjustmentBoundaryPosition = rightOriginX + _skinWidth - EdgeSlideUpCornerWidth;

      var hasHit = false;

      var hasHitOutsideAdjustmentBoundaries = false;

      var hasHitWithinLeftAdjustmentBoundaries = false;

      var hasHitWithinRightAdjustmentBoundaries = false;

      for (var i = 0; i < TotalVerticalRays; i++)
      {
        _topEdgeCollisionTestContainers[i].InsetLeft = leftOriginX + i * _horizontalDistanceBetweenRays;

        _topEdgeCollisionTestContainers[i].IsWithinLeftAdjustmentArea = _topEdgeCollisionTestContainers[i].InsetLeft <= leftAdjustmentBoundaryPosition;

        _topEdgeCollisionTestContainers[i].IsWithinRightAdjustmentArea = _topEdgeCollisionTestContainers[i].InsetLeft >= rightAdjustmentBoundaryPosition;

        var ray = new Vector2(_topEdgeCollisionTestContainers[i].InsetLeft, topOriginY);

        _topEdgeCollisionTestContainers[i].RaycastHit2D = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);

        Logger.Trace(TRACE_TAG, "moveVertically -> test ray origin: {0}, ray target position: {1}, delta: {2}, delta target position: {3}",
          ray,
          new Vector2(rayDirection.x * rayDistance, rayDirection.y * rayDistance),
          moveCalculationResult.DeltaMovement,
          Transform.position + moveCalculationResult.DeltaMovement);

        if (_topEdgeCollisionTestContainers[i].RaycastHit2D)
        {
          hasHit = true;

          if (!hasHitOutsideAdjustmentBoundaries
            && !_topEdgeCollisionTestContainers[i].IsWithinLeftAdjustmentArea
            && !_topEdgeCollisionTestContainers[i].IsWithinRightAdjustmentArea)
          {
            hasHitOutsideAdjustmentBoundaries = true;
          }

          if (!hasHitWithinLeftAdjustmentBoundaries
            && _topEdgeCollisionTestContainers[i].IsWithinLeftAdjustmentArea)
          {
            hasHitWithinLeftAdjustmentBoundaries = true;
          }

          if (!hasHitWithinRightAdjustmentBoundaries
            && _topEdgeCollisionTestContainers[i].IsWithinRightAdjustmentArea)
          {
            hasHitWithinRightAdjustmentBoundaries = true;
          }

          Logger.Trace(TRACE_TAG, "moveVertically -> ray hit; hit point: {0}, new delta: {1}, new delta target position: {2}",
            _topEdgeCollisionTestContainers[i].RaycastHit2D.point,
            moveCalculationResult.DeltaMovement,
            Transform.position + moveCalculationResult.DeltaMovement);

          DrawRay(ray, rayDirection * rayDistance, Color.red);

          var deltaMovementY = _topEdgeCollisionTestContainers[i].RaycastHit2D.point.y - ray.y;

          if (!_topEdgeCollisionTestContainers[i].IsWithinLeftAdjustmentArea
            && !_topEdgeCollisionTestContainers[i].IsWithinRightAdjustmentArea
            && Mathf.Abs(deltaMovementY) < _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
          {
            // direct hit within outside the adjustment area -> no adjustments possible
            moveCalculationResult.DeltaMovement.y = deltaMovementY;
            moveCalculationResult.DeltaMovement.y -= _skinWidth;

            moveCalculationResult.CollisionState.Above = true;

            _raycastHitsThisFrame.Add(_topEdgeCollisionTestContainers[i].RaycastHit2D);

            return;
          }
        }
        else
        {
          DrawRay(ray, rayDirection * rayDistance, Color.blue);
        }
      }

      if (hasHit)
      {
        Logger.Trace(TRACE_TAG, string.Format("hasHit: {0}, hasHitOutsideAdjustmentBoundaries: {1}, hasHitWithinLeftAdjustmentBoundaries: {2}, hasHitWithinRightAdjustmentBoundaries: {3}",
          hasHit,
          hasHitOutsideAdjustmentBoundaries,
          hasHitWithinLeftAdjustmentBoundaries,
          hasHitWithinRightAdjustmentBoundaries));

        Logger.Trace(TRACE_TAG, string.Format("leftOriginX: {0}, leftAdjustmentBoundaryPosition: {1}, rightOriginX: {2}, rightAdjustmentBoundaryPosition: {3}, topOriginY: {4}",
          leftOriginX,
          leftAdjustmentBoundaryPosition,
          rightOriginX,
          rightAdjustmentBoundaryPosition,
          topOriginY));

        if (!hasHitOutsideAdjustmentBoundaries)
        {
          var leftAdjustmentRaycastHit = new RaycastHit2D();

          // first left
          var leftAdjustmentRay = new Vector2(leftAdjustmentBoundaryPosition, topOriginY);

          DrawRay(leftAdjustmentRay, rayDirection * rayDistance, Color.yellow);

          leftAdjustmentRaycastHit = Physics2D.Raycast(leftAdjustmentRay, rayDirection, rayDistance, mask);

          if (!leftAdjustmentRaycastHit
            && !hasHitWithinRightAdjustmentBoundaries)
          {
            Logger.Trace(TRACE_TAG, "moveVertically -> Vert Ray Hit. isGoingUp: {0}, deltaMovement.y: {1}",
              isGoingUp,
              moveCalculationResult.DeltaMovement.y);

            var leftTopRay = new Vector2(
              leftOriginX - SkinWidth + EdgeSlideUpCornerWidth,
              topOriginY + SkinWidth + moveCalculationResult.DeltaMovement.y);

            var raycastHit = Physics2D.Raycast(leftTopRay, -Vector2.right, EdgeSlideUpCornerWidth, mask);

            DrawRay(leftTopRay, -Vector2.right * EdgeSlideUpCornerWidth, Color.magenta);

            Logger.Assert(raycastHit == true,
              string.Format("This should always hit! raytest: {0}; direction: {1}; magnitude: {2}",
              leftTopRay,
              (-Vector2.right),
              EdgeSlideUpCornerWidth));

            Transform.Translate(
              EdgeSlideUpCornerWidth - raycastHit.distance + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR,
              0f,
              0f,
              Space.World);

            return;
          }

          // we reached this line, so do right
          var rightAdjustmentRaycastHit = new RaycastHit2D();

          var rightAdjustmentRay = new Vector2(rightAdjustmentBoundaryPosition, topOriginY);

          DrawRay(rightAdjustmentRay, rayDirection * rayDistance, Color.yellow);

          rightAdjustmentRaycastHit = Physics2D.Raycast(rightAdjustmentRay, rayDirection, rayDistance, mask);

          if (!rightAdjustmentRaycastHit
            && !hasHitWithinLeftAdjustmentBoundaries)
          {
            Logger.Trace(TRACE_TAG, "moveVertically -> Vert Ray Hit. isGoingUp: {0}, deltaMovement.y: {1}",
              isGoingUp,
              moveCalculationResult.DeltaMovement.y);

            var rightTopRay = new Vector2(
              rightOriginX + SkinWidth - EdgeSlideUpCornerWidth,
              topOriginY + SkinWidth + moveCalculationResult.DeltaMovement.y);

            var raycastHit = Physics2D.Raycast(rightTopRay, Vector2.right, EdgeSlideUpCornerWidth, mask);

            DrawRay(rightTopRay, -Vector2.right * EdgeSlideUpCornerWidth, Color.magenta);

            Logger.Assert(raycastHit == true,
              string.Format("This should always hit! raytest: {0}; direction: {1}; magnitude: {2}",
              rightTopRay,
              (Vector2.right),
              EdgeSlideUpCornerWidth));

            Transform.Translate(
              -(EdgeSlideUpCornerWidth - raycastHit.distance + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR),
              0f,
              0f,
              Space.World);

            return;
          }
        }

        // we reached this line, do normal vert ray checks
        for (var i = 0; i < TotalVerticalRays; i++)
        {
          Logger.Trace(TRACE_TAG, _topEdgeCollisionTestContainers[i]);

          if (_topEdgeCollisionTestContainers[i].RaycastHit2D)
          {
            Logger.Trace(TRACE_TAG, "moveVertically -> Vert Ray Hit. isGoingUp: {0}, deltaMovement.y: {1}",
              isGoingUp,
              moveCalculationResult.DeltaMovement.y);

            // set our new deltaMovement and recalculate the rayDistance taking it into account
            moveCalculationResult.DeltaMovement.y =
              _topEdgeCollisionTestContainers[i].RaycastHit2D.point.y - topOriginY;

            Logger.Trace(TRACE_TAG, "moveVertically -> ray hit; hit point: {0}, new delta: {1}, new delta target position: {2}",
              _topEdgeCollisionTestContainers[i].RaycastHit2D.point,
              moveCalculationResult.DeltaMovement,
              Transform.position + moveCalculationResult.DeltaMovement);

            rayDistance = Mathf.Abs(moveCalculationResult.DeltaMovement.y);

            // remember to remove the skinWidth from our deltaMovement
            moveCalculationResult.DeltaMovement.y -= _skinWidth;

            moveCalculationResult.CollisionState.Above = true;

            _raycastHitsThisFrame.Add(_topEdgeCollisionTestContainers[i].RaycastHit2D);

            // we add a small fudge factor for the float operations here. if our rayDistance is smaller
            // than the width + fudge bail out because we have a direct impact
            if (rayDistance < _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
            {
              break;
            }
          }
        }
      }
    }
    else
    {
      var initialRayOrigin = isGoingUp
        ? _raycastOrigins.TopLeft
        : _raycastOrigins.BottomLeft;

      // apply our horizontal deltaMovement here so that we do our raycast from the actual 
      // position we would be in if we had moved
      initialRayOrigin.x += moveCalculationResult.DeltaMovement.x;

      RaycastHit2D raycastHit;

      for (var i = 0; i < TotalVerticalRays; i++)
      {
        var ray = new Vector2(initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y);

        DrawRay(ray, rayDirection * rayDistance, Color.red);

        raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);

        Logger.Trace(TRACE_TAG, "moveVertically -> test ray origin: {0}, ray target position: {1}, delta: {2}, delta target position: {3}",
          ray,
          new Vector2(rayDirection.x * rayDistance, rayDirection.y * rayDistance),
          moveCalculationResult.DeltaMovement,
          Transform.position + moveCalculationResult.DeltaMovement);

        if (raycastHit)
        {
          Logger.Trace(TRACE_TAG, "moveVertically -> Vert Ray Hit. isGoingUp: {0}, deltaMovement.y: {1}",
            isGoingUp,
            moveCalculationResult.DeltaMovement.y);

          // set our new deltaMovement and recalculate the rayDistance taking it into account
          moveCalculationResult.DeltaMovement.y = raycastHit.point.y - ray.y;

          Logger.Trace(TRACE_TAG, "moveVertically -> ray hit; hit point: {0}, new delta: {1}, new delta target position: {2}",
            raycastHit.point,
            moveCalculationResult.DeltaMovement,
            Transform.position + moveCalculationResult.DeltaMovement);

          rayDistance = Mathf.Abs(moveCalculationResult.DeltaMovement.y);

          // remember to remove the skinWidth from our deltaMovement
          if (isGoingUp)
          {
            moveCalculationResult.DeltaMovement.y -= _skinWidth;

            moveCalculationResult.CollisionState.Above = true;
          }
          else
          {
            moveCalculationResult.DeltaMovement.y += _skinWidth;

            moveCalculationResult.CollisionState.Below = true;

            moveCalculationResult.CollisionState.LastTimeGrounded = Time.time;
          }

          _raycastHitsThisFrame.Add(raycastHit);

          // this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
          // where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
          if (!isGoingUp && moveCalculationResult.DeltaMovement.y > 0.00001f)
          {
            moveCalculationResult.IsGoingUpSlope = true;
          }

          // we add a small fudge factor for the float operations here. if our rayDistance is smaller
          // than the width + fudge bail out because we have a direct impact
          if (rayDistance < _skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
          {
            break;
          }
        }
      }
    }
  }

  /// <summary>
  /// checks the center point under the BoxCollider2D for a slope. If it finds one then the deltaMovement is adjusted so that
  /// the player stays grounded and the slopeSpeedModifier is taken into account to speed up movement.
  /// </summary>
  private void HandleVerticalSlope(ref MoveCalculationResult moveCalculationResult)
  {
    // slope check from the center of our collider
    var centerOfCollider = (_raycastOrigins.BottomLeft.x + _raycastOrigins.BottomRight.x) * 0.5f;

    var rayDirection = -Vector2.up;

    // the ray distance is based on our slopeLimit
    var slopeCheckRayDistance = _slopeLimitTangent * (_raycastOrigins.BottomRight.x - centerOfCollider);

    var slopeRay = new Vector2(centerOfCollider, _raycastOrigins.BottomLeft.y);

    DrawRay(slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow);

    var raycastHit = Physics2D.Raycast(slopeRay, rayDirection, slopeCheckRayDistance, PlatformMask);

    if (raycastHit)
    {
      // bail out if we have no slope
      var angle = Vector2.Angle(raycastHit.normal, Vector2.up);

      if (angle == 0)
      {
        return;
      }

      // we are moving down the slope if our normal and movement direction are in the same x direction
      var isMovingDownSlope =
        Mathf.Sign(raycastHit.normal.x) == Mathf.Sign(moveCalculationResult.DeltaMovement.x);

      if (isMovingDownSlope)
      {
        // going down we want to speed up in most cases so the slopeSpeedMultiplier curve should be > 1 for negative angles
        var slopeModifier = (SlopeSpeedMultiplierOverride == null ? SlopeSpeedMultiplier : SlopeSpeedMultiplierOverride).Evaluate(-angle);

        Logger.Trace(TRACE_TAG, "HandleVerticalSlope -> moving down, slope modifier: {0}, angle: {1}, slopeSpeedMultiplierOverride == null: {2}",
          slopeModifier,
          angle,
          SlopeSpeedMultiplierOverride == null);

        // we add the extra downward movement here to ensure we "stick" to the surface below
        moveCalculationResult.DeltaMovement.y += raycastHit.point.y - slopeRay.y - SkinWidth;

        moveCalculationResult.DeltaMovement.x *= slopeModifier;

        moveCalculationResult.CollisionState.MovingDownSlope = true;

        moveCalculationResult.CollisionState.SlopeAngle = angle;
      }
    }
  }

  [System.Diagnostics.Conditional("DEBUG")]
  private void DrawRay(Vector3 start, Vector3 dir, Color color)
  {
    Debug.DrawRay(start, dir, color);
  }

  private struct CharacterRaycastOrigins
  {
    public Vector3 TopLeft;

    public Vector3 BottomRight;

    public Vector3 BottomLeft;
  }

  struct TopEdgeCollisionTestContainer
  {
    public RaycastHit2D RaycastHit2D;

    public bool IsWithinLeftAdjustmentArea;

    public bool IsWithinRightAdjustmentArea;

    public float InsetLeft;

    public override string ToString()
    {
      return string.Format("raycastHit2D: {0}, insetLeft: {1}, isWithinLeftAdjustmentArea: {2}, isWithinRightAdjustmentArea: {3}",
        RaycastHit2D == true ? "true" : "false",
        InsetLeft,
        IsWithinLeftAdjustmentArea,
        IsWithinRightAdjustmentArea);
    }
  }

}
