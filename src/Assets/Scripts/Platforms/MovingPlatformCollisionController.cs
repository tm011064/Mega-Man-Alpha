using UnityEngine;

public class MovingPlatformCollisionController : MonoBehaviour
{
  private const string TRACE_TAG = "MovingPlatformCollisionController";

  private const float FUDGE_FACTOR = .0001f;

  private BoxCollider2D _boxCollider;

  private BoxCollider2D _playerBoxCollider;

  private PlayerController _playerController;

  private AttachPlayerControllerToObject _attachPlayerControllerToObject;

  void Start()
  {
    _playerController = GameManager.Instance.Player;
    _boxCollider = GetComponent<BoxCollider2D>();
    _playerBoxCollider = _playerController.EnvironmentBoxCollider;
  }

  void LateUpdate()
  {
    if (_boxCollider.bounds.Intersects(_playerBoxCollider.bounds))
    {
      if (_playerController.CurrentPlatform != gameObject)
      {
        // this check is to find out whether character is standing on this object. If so, no need to perform any additional checks
        // Note: we assume that the player was hit by a horizontal move

        var topLeft = new Vector2(_boxCollider.bounds.center.x - _boxCollider.bounds.extents.x, _boxCollider.bounds.center.y + _boxCollider.bounds.extents.y);

        var bottomRight = new Vector2(_boxCollider.bounds.center.x + _boxCollider.bounds.extents.x, _boxCollider.bounds.center.y - _boxCollider.bounds.extents.y);

        var playerTopLeft = new Vector2(_playerBoxCollider.bounds.center.x - _playerBoxCollider.bounds.extents.x, _playerBoxCollider.bounds.center.y + _playerBoxCollider.bounds.extents.y);

        var playerBottomRight = new Vector2(_playerBoxCollider.bounds.center.x + _playerBoxCollider.bounds.extents.x, _playerBoxCollider.bounds.center.y - _playerBoxCollider.bounds.extents.y);

        if (playerBottomRight.x > topLeft.x
          && playerBottomRight.x < bottomRight.x)
        {
          // right side of player inside
          Logger.Trace(
            TRACE_TAG,
            "LEFT Collision. Current player position: " + _playerController.transform.position
              + ", bounds: " + _boxCollider.bounds
              + ", bounds player: " + _playerBoxCollider.bounds);

          _playerController.transform.position = new Vector3(
            (_boxCollider.gameObject.transform.position).x
            - _playerController.EnvironmentBoxCollider.size.x / 2
            - FUDGE_FACTOR,
            _playerController.transform.position.y,
            _playerController.transform.position.z);

          Logger.Trace(TRACE_TAG, "Adjusted player position: " + _playerController.transform.position);
        }
        else if (playerTopLeft.x < bottomRight.x
              && playerTopLeft.x > topLeft.x)
        {
          Logger.Trace(
            TRACE_TAG,
            "RIGHT Collision. Current player position: " + _playerController.transform.position
              + ", bounds: " + _boxCollider.bounds
              + ", bounds player: " + _playerBoxCollider.bounds);

          _playerController.transform.position = new Vector3(
            (_boxCollider.gameObject.transform.position + _boxCollider.offset.ToVector3() + _boxCollider.offset.ToVector3()).x
            + _playerController.EnvironmentBoxCollider.size.x / 2
            + FUDGE_FACTOR,
            _playerController.transform.position.y,
            _playerController.transform.position.z);

          Logger.Trace(TRACE_TAG, "Adjusted player position: " + _playerController.transform.position);
        }
        else
        {
          Logger.Trace(
            TRACE_TAG,
            "NO OVERLAP Collision. Current player position: " + _playerController.transform.position
              + ", bounds: " + _boxCollider.bounds
              + ", bounds player: " + _playerBoxCollider.bounds);
        }
      }
    }
  }
}
