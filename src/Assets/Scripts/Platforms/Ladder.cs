using UnityEngine;

public class Ladder : MonoBehaviour
{
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
      && _gameManager.Player.BoxCollider.bounds.AreWithinVerticalShaftOf(_boxCollider.bounds))
    {
      _topEdgeCollider.enabled = false;

      _gameManager.Player.PlayerState |= PlayerState.ClimbingLadder;

      CreateAndPushStartClimbDownLadderControlHandler();

      return;
    }

    if (_hasPlayerEntered
      && (_gameManager.Player.PlayerState & PlayerState.ClimbingLadder) == 0
      && verticalAxisState.Value > 0f
      && _gameManager.Player.BoxCollider.bounds.AreWithinVerticalShaftOf(_boxCollider.bounds))
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
      _topEdge.transform.position.y);

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
      _topEdge == null
        ? _boxCollider.bounds.center.y + _boxCollider.bounds.extents.y
        : _topEdge.transform.position.y);

    _gameManager.Player.PushControlHandler(ladderClimbControlHandler);
  }
}
