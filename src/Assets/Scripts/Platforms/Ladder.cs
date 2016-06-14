using UnityEngine;

public class Ladder : MonoBehaviour
{
  private float _topEdgeVerticalOffsetFromBoxCollider = 0f;

  private GameManager _gameManager;

  private bool _hasPlayerEntered;

  private BoxCollider2D _boxCollider;

  private GameObject _topEdge;

  private Collider2D _topEdgeCollider;

  void Awake()
  {
    _gameManager = GameManager.Instance;

    _boxCollider = GetComponent<BoxCollider2D>();

    var topEdgeTransform = transform.FindChild("TopEdge");

    if (topEdgeTransform != null)
    {
      _topEdge = topEdgeTransform.gameObject;

      _topEdgeCollider = _topEdge.GetComponent<EdgeCollider2D>();

      if (_topEdgeCollider == null)
      {
        throw new MissingComponentException("A " + typeof(Ladder).Name + " game object's TopEdge object must contain a "
          + typeof(EdgeCollider2D).Name + " component");
      }

      _topEdgeVerticalOffsetFromBoxCollider =
        _topEdge.transform.position.y
        - _boxCollider.bounds.center.y
        - _boxCollider.bounds.extents.y;
    }
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    if (col.gameObject == _gameManager.Player.gameObject)
    {
      _hasPlayerEntered = true;
    }
  }

  void OnTriggerExit2D(Collider2D col)
  {
    if (col.gameObject == _gameManager.Player.gameObject)
    {
      _hasPlayerEntered = false;
    }
  }

  void Update()
  {
    if (!_gameManager.Player.ClimbSettings.EnableLadderClimbing)
    {
      return;
    }

    var verticalAxisState = _gameManager.InputStateManager.GetAxisState("Vertical");

    if (_gameManager.Player.CurrentPlatform != null
      && _gameManager.Player.CurrentPlatform == _topEdge
      && verticalAxisState.Value < 0f
      && _gameManager.Player.BoxCollider.bounds.AreWithinVerticalBoundsOf(_boxCollider.bounds))
    {
      _topEdgeCollider.enabled = false;

      _gameManager.Player.PlayerState |= PlayerState.ClimbingLadder;

      CreateAndPushStartClimbDownLadderControlHandler();

      return;
    }

    if (_hasPlayerEntered
      && (_gameManager.Player.PlayerState & PlayerState.ClimbingLadder) == 0
      && verticalAxisState.Value > 0f
      && _gameManager.Player.BoxCollider.bounds.AreWithinVerticalBoundsOf(_boxCollider.bounds))
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
      _boxCollider.bounds,
      _boxCollider.bounds.center.y + _boxCollider.bounds.extents.y + _topEdgeVerticalOffsetFromBoxCollider);

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
      _boxCollider.bounds,
      _boxCollider.bounds.center.y + _boxCollider.bounds.extents.y + _topEdgeVerticalOffsetFromBoxCollider);

    _gameManager.Player.PushControlHandler(ladderClimbControlHandler);
  }
}
