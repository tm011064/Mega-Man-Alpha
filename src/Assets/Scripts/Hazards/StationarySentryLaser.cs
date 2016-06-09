using UnityEngine;

public class StationarySentryLaser : SpawnBucketItemBehaviour
{
  [Tooltip("All layers that the scan rays can collide with. Should include platforms and player.")]
  public LayerMask ScanRayCollisionLayers = 0;

  [Tooltip("Scan rays are sent out in all directions. The start angle defines the start boundary of the line of sight in degrees. 0 degrees means right direction (1,0), going counter clockwise. Example: 0 = (1, 0), 90 = (0, 1), 180 = (-1, 0)")]
  public float ScanRayStartAngle = 0f;

  [Tooltip("Scan rays are sent out in all directions. The end angle defines the end boundary of the line of sight in degrees. 0 degrees means right direction (1,0), going counter clockwise. Example: 0 = (1, 0), 90 = (0, 1), 180 = (-1, 0)")]
  public float ScanRayEndAngle = 360f;

  [Tooltip("The length of the scan rays emitted from the position of this gameobject.")]
  public float ScanRayLength = 1280;

  [Tooltip("Once the sentry laser detects the player, he has some time to get cover before being killed by the laser. This is the variable indicating the time.")]
  public float TimeNeededToKillPlayer = .1f;

  private LineRenderer _lineRenderer;

  private float _startAngleRad;

  private float _endAngleRad;

  private float _rateOfFireInterval;

  private float _playerInSightDuration;

  private ObjectPoolingManager _objectPoolingManager;

  private PlayerController _playerController;

  void Awake()
  {
    _lineRenderer = GetComponent<LineRenderer>();

    _startAngleRad = ScanRayStartAngle * Mathf.Deg2Rad;

    _endAngleRad = ScanRayEndAngle * Mathf.Deg2Rad;
  }

  void OnEnable()
  {
    _playerController = GameManager.Instance.Player;

    Logger.Info("Enabled stationary sentry laser " + GetHashCode());
  }

  void Update()
  {
    var isSeeingPlayer = false;

    var playerVector = _playerController.transform.position - transform.position;

    var angle = Mathf.Atan2(playerVector.y, playerVector.x);

    if (angle < 0f)
    {
      angle += 2 * Mathf.PI;
    }

    if (angle >= _startAngleRad && angle <= _endAngleRad)
    {
      var raycastHit = Physics2D.Raycast(gameObject.transform.position, playerVector.normalized, playerVector.magnitude, ScanRayCollisionLayers);

      if (raycastHit && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
      {
        isSeeingPlayer = true;

        _playerInSightDuration += Time.deltaTime;
      }
    }

    DrawRay(gameObject.transform.position, playerVector, isSeeingPlayer ? Color.red : Color.gray);

    if (!isSeeingPlayer)
    {
      _playerInSightDuration = 0f;
    }

    if (isSeeingPlayer)
    {
      if (!_lineRenderer.enabled)
      {
        _lineRenderer.enabled = true;
      }

      if (_lineRenderer.useWorldSpace)
      {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _playerController.transform.position);
      }
      else
      {
        _lineRenderer.SetPosition(0, Vector3.zero);
        _lineRenderer.SetPosition(1, _playerController.transform.position - transform.position);
      }
    }
    else
    {
      _lineRenderer.enabled = false;
    }

    if (_playerInSightDuration >= TimeNeededToKillPlayer)
    {
      GameManager.Instance.PowerUpManager.KillPlayer();
    }
  }

  [System.Diagnostics.Conditional("DEBUG")]
  private void DrawRay(Vector3 start, Vector3 dir, Color color)
  {
    Debug.DrawRay(start, dir, color);
  }
}
