using UnityEngine;

public class Ladder : MonoBehaviour
{
  [Tooltip("This is the vertical distance that the player travels while hamstering over the top edge of the ladder")]
  public float ClimbOverTopExtension = 32f;

  private GameManager _gameManager;

  private bool _hasPlayerEntered;

  private BoxCollider2D _boxCollider;

  void Awake()
  {
    _gameManager = GameManager.Instance;
    _boxCollider = GetComponent<BoxCollider2D>();
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
    if (!_hasPlayerEntered)
    {
      return;
    }

    if ((_gameManager.Player.PlayerState & PlayerState.ClimbingLadder) == 0)
    {
      if (_gameManager.InputStateManager.GetAxisState("Vertical").Value > 0f
        && _gameManager.Player.BoxCollider.bounds.AreWithinVerticalBoundsOf(_boxCollider.bounds))
      {
        _gameManager.Player.PlayerState |= PlayerState.ClimbingLadder;

        CreateAndPushLadderClimbControlHandler();
      }
    }
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
      _boxCollider.bounds.center.y + _boxCollider.bounds.extents.y + ClimbOverTopExtension);

    _gameManager.Player.PushControlHandler(ladderClimbControlHandler);
  }
}
