using UnityEngine;

public class TranslateTransformAction
{
  private readonly Vector3 _targetPosition;

  private readonly Transform _transform;

  private readonly Easing _easing;

  private readonly EasingType _easingType;

  private readonly float _duration;

  private float _startTime;

  private TranslateTransformActionStatus _actionStatus = TranslateTransformActionStatus.Idle;

  private Vector3 _path;

  private Vector3 _startPosition;

  public TranslateTransformAction(
    Transform transform,
    Vector3 targetPosition,
    float duration,
    EasingType easingType,
    Easing easing)
  {
    _easing = easing;
    _easingType = easingType;
    _transform = transform;
    _targetPosition = targetPosition;
    _duration = duration;
  }

  public void Start()
  {
    _actionStatus = TranslateTransformActionStatus.Started;

    _startTime = Time.time;

    _startPosition = _transform.position;

    _path = _targetPosition - _startPosition;
  }

  public TranslateTransformActionStatus Update()
  {
    if (_actionStatus != TranslateTransformActionStatus.Started)
    {
      return _actionStatus;
    }

    float currentTime = Time.time - _startTime;

    var percentage = _easing.GetValue(_easingType, currentTime, _duration);

    if (percentage >= 1f)
    {
      _transform.position = _targetPosition;

      _actionStatus = TranslateTransformActionStatus.Completed;
    }
    else
    {
      var translationVector = _startPosition + (_path.normalized * (_path.magnitude * percentage));

      _transform.position = translationVector;
    }

    return _actionStatus;
  }
}
