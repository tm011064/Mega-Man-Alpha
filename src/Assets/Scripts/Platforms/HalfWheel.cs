using System.Collections.Generic;
using UnityEngine;

public partial class HalfWheel : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public GameObject FloatingAttachedPlatform;

  public float Radius = 256f;

  public float MoveDuration = 2f;

  public float StopDuration = 2f;

  public Direction StartDirection = Direction.Right;

  public EasingType EasingType = EasingType.Linear;

  private GameObject _platform;

  private float _currentAngle;

  private float _currentMoveDuration;

  private Direction _currentDirection = Direction.Right;

  private float _nextStartTime;

  private Easing _easing = new Easing();

  private bool _isPlayerAttached;

  private BoxCollider2D _visibilityCollider;

  private ObjectPoolingManager _objectPoolingManager;

  void Update()
  {
    if (Time.time < _nextStartTime)
    {
      return;
    }

    _currentMoveDuration += Time.deltaTime;

    if (_currentDirection == Direction.Left)
    {
      _currentAngle = _easing.GetValue(EasingType, _currentMoveDuration, MoveDuration) * -Mathf.PI;
    }
    else
    {
      _currentAngle = -Mathf.PI + _easing.GetValue(EasingType, _currentMoveDuration, MoveDuration) * Mathf.PI;
    }

    if (_currentMoveDuration >= MoveDuration)
    {
      _nextStartTime = Time.time + StopDuration;

      _currentMoveDuration = 0f;

      if (_currentDirection == Direction.Right)
      {
        _currentAngle = 0;
        _currentDirection = Direction.Left;
      }
      else
      {
        _currentAngle = -Mathf.PI;
        _currentDirection = Direction.Right;
      }
    }

    var initial = new Vector3(
      transform.position.x + Radius,
      transform.position.y,
      transform.position.z);

    var rotated = new Vector3(
      Mathf.Cos(_currentAngle) * (initial.x - transform.position.x) - Mathf.Sin(_currentAngle) * (initial.y - transform.position.y) + transform.position.x,
      Mathf.Sin(_currentAngle) * (initial.x - transform.position.x) + Mathf.Cos(_currentAngle) * (initial.y - transform.position.y) + transform.position.y,
      transform.position.z);

    _platform.transform.position = rotated;

    if (_currentMoveDuration == 0f)
    {
      Debug.Log("Angle: " + _currentAngle * Mathf.Rad2Deg + " (rad: " + _currentAngle + ")" + ", platform pos: " + _platform.transform.position);
    }
  }

  void OnEnable()
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    Logger.Info("Enabling half wheel " + name);

    var platform = _objectPoolingManager.GetObject(FloatingAttachedPlatform.name);

    _currentAngle = StartDirection == Direction.Right
      ? -Mathf.PI
      : 0f;

    Vector3 initial = new Vector3(
      transform.position.x + Radius,
      transform.position.y,
      transform.position.z);

    Vector3 rotated = new Vector3(
      Mathf.Cos(_currentAngle) * (initial.x - transform.position.x) - Mathf.Sin(_currentAngle) * (initial.y - transform.position.y) + transform.position.x,
      Mathf.Sin(_currentAngle) * (initial.x - transform.position.x) + Mathf.Cos(_currentAngle) * (initial.y - transform.position.y) + transform.position.y,
      transform.position.z);

    platform.transform.position = rotated;

    _platform = platform;

    _currentDirection = StartDirection;

    _nextStartTime = Time.time + StopDuration;
  }

  void OnDisable()
  {
    Logger.Info("Disabling half wheel " + name);

    _objectPoolingManager.Deactivate(_platform);
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(FloatingAttachedPlatform);
  }
}
