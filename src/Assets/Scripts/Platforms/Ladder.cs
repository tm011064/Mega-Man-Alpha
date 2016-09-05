using UnityEngine;

public partial class Ladder : MonoBehaviour
{
  [Tooltip("The width acts as a shaft that the player needs to be within so he can trigger the climb state while pressing the up button")]
  public Vector2 Size;

  [Tooltip("When climbing up, this is the distance threshold which triggers the player over ladder top pull up and animation")]
  public float LadderTopAnimationStartDistance = 16f;

  private GameManager _gameManager;

  private GameObject _topEdge;

  private EdgeCollider2D _topEdgeCollider;

  private Vector2 _extents;

  void Awake()
  {
    _gameManager = GameManager.Instance;

    _extents = new Vector2(Size.x * .5f, Size.y * .5f);

    _topEdge = this.GetOrCreateChildGameObject("Top Collider", "OneWayPlatform");

    _topEdge.hideFlags = HideFlags.NotEditable;

    _topEdgeCollider = _topEdge.GetComponent<EdgeCollider2D>();

    if (_topEdgeCollider == null)
    {
      _topEdgeCollider = _topEdge.AddComponent<EdgeCollider2D>();
    }

    _topEdgeCollider.hideFlags = HideFlags.NotEditable;
    _topEdgeCollider.isTrigger = true;
    _topEdgeCollider.enabled = true;
    _topEdgeCollider.points = new Vector2[]
      {
        new Vector2(-_extents.x, _extents.y),
        new Vector2(_extents.x, _extents.y)
      };
  }

  private bool IsPlayerBetweenVerticalColliders()
  {
    return _gameManager.Player.EnvironmentBoxCollider.bounds.min.x >= transform.position.x - _extents.x
      && _gameManager.Player.EnvironmentBoxCollider.bounds.max.x <= transform.position.x + _extents.x;
  }

  private bool IsPlayerTopAboveBottomCollider()
  {
    return _gameManager.Player.EnvironmentBoxCollider.bounds.max.y > transform.position.y - _extents.y;
  }

  private bool IsPlayerTopBelowTopBoundary()
  {
    return _gameManager.Player.EnvironmentBoxCollider.bounds.max.y <
      transform.position.y + LadderTopAnimationStartDistance + _extents.y;
  }

  private bool TriggeredClimbDownFromEdge(AxisState verticalAxisState)
  {
    return (_gameManager.Player.PlayerState & PlayerState.ClimbingLadder) == 0
      && (_gameManager.Player.PlayerState & PlayerState.Sliding) == 0
      && (_gameManager.Player.PlayerState & PlayerState.Locked) == 0
      && _gameManager.Player.CurrentPlatform != null
      && _gameManager.Player.CurrentPlatform == _topEdge
      && verticalAxisState.Value < 0f
      && IsPlayerBetweenVerticalColliders();
  }

  private bool TriggeredClimbUp(AxisState verticalAxisState)
  {
    return (_gameManager.Player.PlayerState & PlayerState.ClimbingLadder) == 0
      && (_gameManager.Player.PlayerState & PlayerState.Locked) == 0
      && verticalAxisState.Value > 0f
      && IsPlayerBetweenVerticalColliders()
      && IsPlayerTopAboveBottomCollider()
      && IsPlayerTopBelowTopBoundary();
  }

  void Update()
  {
    if (!_gameManager.Player.ClimbSettings.EnableLadderClimbing)
    {
      return;
    }

    var verticalAxisState = _gameManager.InputStateManager.GetVerticalAxisState();

    if (TriggeredClimbDownFromEdge(verticalAxisState))
    {
      _topEdgeCollider.enabled = false;

      _gameManager.Player.PlayerState |= PlayerState.ClimbingLadder;

      CreateAndPushStartClimbDownLadderControlHandler();

      return;
    }

    if (TriggeredClimbUp(verticalAxisState))
    {
      _gameManager.Player.PlayerState |= PlayerState.ClimbingLadder;

      CreateAndPushLadderClimbControlHandler();
    }
  }

  private void CreateAndPushStartClimbDownLadderControlHandler()
  {
    _gameManager.Player.transform.position = new Vector3(
      gameObject.transform.position.x,
      _gameManager.Player.transform.position.y,
      _gameManager.Player.transform.position.z);

    var controlHandler = new StartClimbDownLadderControlHandler(
      _gameManager.Player,
      transform,
      _extents,
      LadderTopAnimationStartDistance);

    controlHandler.Disposed += OnStartClimbDownLadderControlHandlerDisposed;

    _gameManager.Player.PushControlHandler(controlHandler);
  }

  void OnStartClimbDownLadderControlHandlerDisposed(StartClimbDownLadderControlHandler controlHandler)
  {
    _topEdgeCollider.enabled = true;

    controlHandler.Disposed -= OnStartClimbDownLadderControlHandlerDisposed;
  }

  private void CreateAndPushLadderClimbControlHandler()
  {
    _gameManager.Player.transform.position = new Vector3(
      gameObject.transform.position.x,
      _gameManager.Player.transform.position.y,
      _gameManager.Player.transform.position.z);

    var ladderClimbControlHandler = new LadderClimbControlHandler(
      _gameManager.Player,
      transform,
      _extents,
      LadderTopAnimationStartDistance);

    _gameManager.Player.PushControlHandler(ladderClimbControlHandler);
  }
}
