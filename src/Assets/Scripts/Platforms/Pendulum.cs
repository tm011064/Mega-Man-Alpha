using System.Collections.Generic;
using UnityEngine;

public partial class Pendulum : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public GameObject FloatingAttachedPlatform;

  public float Radius = 256f;

  public float MoveDuration = 2f;

  public float StopDuration = 2f;

  public EasingType EasingType = EasingType.Linear;

  public float StartAngle;

  public float EndAngle;

  private GameObject _platform;

  private float _startAngleRad;

  private float _endAngleRad;

  private float _totalAngleRad;

  private float _currentAngle;

  private float _currentMoveDuration;

  private float _directionFactor;

  private bool _isMovingTowardsEndPoint;

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

    if (_isMovingTowardsEndPoint)
    {
      _currentAngle =
        _startAngleRad
        + _easing.GetValue(EasingType, _currentMoveDuration, MoveDuration)
        * _totalAngleRad
        * _directionFactor;
    }
    else
    {
      _currentAngle =
        _endAngleRad
        - _easing.GetValue(EasingType, _currentMoveDuration, MoveDuration)
        * _totalAngleRad
        * _directionFactor;
    }

    if (_currentMoveDuration >= MoveDuration)
    {
      _nextStartTime = Time.time + StopDuration;

      _currentMoveDuration = 0f;

      if (_isMovingTowardsEndPoint)
      {
        _currentAngle = _endAngleRad;
      }
      else
      {
        _currentAngle = _startAngleRad;
      }

      _isMovingTowardsEndPoint = !_isMovingTowardsEndPoint;
    }

    var initial = new Vector3(transform.position.x + Radius, transform.position.y, transform.position.z);

    var rotated = new Vector3(
      Mathf.Cos(_currentAngle) * (initial.x - transform.position.x) - Mathf.Sin(_currentAngle) * (initial.y - transform.position.y) + transform.position.x,
      Mathf.Sin(_currentAngle) * (initial.x - transform.position.x) + Mathf.Cos(_currentAngle) * (initial.y - transform.position.y) + transform.position.y,
      transform.position.z);

    _platform.transform.position = rotated;
  }

  void OnEnable()
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    _startAngleRad = StartAngle * Mathf.Deg2Rad;

    _endAngleRad = EndAngle * Mathf.Deg2Rad;

    _totalAngleRad = Mathf.Abs(_endAngleRad - _startAngleRad);

    _isMovingTowardsEndPoint = true;

    _directionFactor = _startAngleRad > _endAngleRad
      ? -1f
      : 1f;

    var platform = _objectPoolingManager.GetObject(FloatingAttachedPlatform.name);

    _currentAngle = _startAngleRad;

    var initial = new Vector3(transform.position.x + Radius, transform.position.y, transform.position.z);

    var rotated = new Vector3(
      Mathf.Cos(_currentAngle) * (initial.x - transform.position.x) - Mathf.Sin(_currentAngle) * (initial.y - transform.position.y) + transform.position.x,
      Mathf.Sin(_currentAngle) * (initial.x - transform.position.x) + Mathf.Cos(_currentAngle) * (initial.y - transform.position.y) + transform.position.y,
      transform.position.z);

    platform.transform.position = rotated;

    _platform = platform;

    _nextStartTime = Time.time + StopDuration;
  }

  void OnDisable()
  {
    _objectPoolingManager.Deactivate(_platform);
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(FloatingAttachedPlatform);
  }
}

