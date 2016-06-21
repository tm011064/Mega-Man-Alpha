using UnityEngine;

public partial class PlayerMetricsRenderer : MonoBehaviour
{
#if UNITY_EDITOR
  public float WalkingJumpDistance = 512f;

  public float RunningJumpDistance = 1024f;

  public float JumpHeight = 360f;

  public float MaxJumpHeight = 304f;

  public float ComfortableJumpHeight = 272f;

  public float WalkingJumpHeightWidth = 332f;

  public Vector2 PlayerDimensions = new Vector2(86, 86);

  public Vector2 SafeAreaBounds = new Vector2(1920, 1080);

  private int _jumpRadiusResolution = 32;

  private Vector3[] _walkingJumpRadiusPositions = null;

  private Vector3[] _runningJumpRadiusPositions = null;

  private BoxCollider2D _collider = null;

  void OnReset()
  {
    _walkingJumpRadiusPositions = null;

    _runningJumpRadiusPositions = null;
  }

  void Start()
  {
    _collider = GetComponent<BoxCollider2D>();

    Logger.Info("Player Dimensions: " + PlayerDimensions);

    if (_collider != null)
    {
      PlayerDimensions = new Vector2(_collider.bounds.extents.x * 2f, _collider.bounds.extents.y * 2f);

      Logger.Info("Player Dimensions: " + PlayerDimensions);
    }
  }

  void OnDrawGizmos()
  {
    if (_collider != null
      || (Application.isEditor && !Application.isPlaying))
    {
      _walkingJumpRadiusPositions = null;

      _runningJumpRadiusPositions = null;

      Gizmos.color = Color.gray;
      if (_walkingJumpRadiusPositions == null)
      {
        _walkingJumpRadiusPositions = GizmoUtility.CreateEllipse(
          WalkingJumpDistance,
          JumpHeight,
          0,
          0,
          0f,
          _jumpRadiusResolution);
      }
      if (_runningJumpRadiusPositions == null)
      {
        _runningJumpRadiusPositions = GizmoUtility.CreateEllipse(
          RunningJumpDistance,
          JumpHeight,
          0,
          0,
          0f,
          _jumpRadiusResolution);
      }

      for (var i = 1; i < _walkingJumpRadiusPositions.Length; i++)
      {
        Gizmos.DrawLine(_walkingJumpRadiusPositions[i - 1] + transform.position, _walkingJumpRadiusPositions[i] + transform.position);
      }

      Gizmos.DrawLine(_walkingJumpRadiusPositions[_walkingJumpRadiusPositions.Length - 1] + transform.position, _walkingJumpRadiusPositions[0] + transform.position);

      for (var i = 1; i < _runningJumpRadiusPositions.Length; i++)
      {
        Gizmos.DrawLine(_runningJumpRadiusPositions[i - 1] + transform.position, _runningJumpRadiusPositions[i] + transform.position);
      }

      Gizmos.DrawLine(_runningJumpRadiusPositions[_runningJumpRadiusPositions.Length - 1] + transform.position, _runningJumpRadiusPositions[0] + transform.position);

      var comfortableJumpHeightYPos = transform.position.y + ComfortableJumpHeight;// -playerDimensions.y * .5f;

      var maxJumpHeightYPos = transform.position.y + MaxJumpHeight;//  - playerDimensions.y * .5f;

      Gizmos.color = Color.gray;

      Gizmos.DrawLine(
        new Vector3(transform.position.x - WalkingJumpHeightWidth * .4f, maxJumpHeightYPos, transform.position.z),
        new Vector3(transform.position.x + WalkingJumpHeightWidth * .4f, maxJumpHeightYPos, transform.position.z));

      Gizmos.color = Color.white;

      Gizmos.DrawLine(
        new Vector3(transform.position.x - WalkingJumpHeightWidth * .7f, comfortableJumpHeightYPos, transform.position.z),
        new Vector3(transform.position.x + WalkingJumpHeightWidth * .7f, comfortableJumpHeightYPos, transform.position.z));

      Gizmos.DrawLine(
        new Vector3(transform.position.x - PlayerDimensions.x, transform.position.y - PlayerDimensions.y * .5f, transform.position.z),
        new Vector3(transform.position.x + PlayerDimensions.x, transform.position.y - PlayerDimensions.y * .5f, transform.position.z));

      // draw player visible rect
      GizmoUtility.DrawBoundingBox(transform.position, SafeAreaBounds, Color.blue);
    }
  }
#endif
}
