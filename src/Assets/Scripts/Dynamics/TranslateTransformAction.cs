using UnityEngine;

public class TranslateTransformAction
{
  public TranslateTransformActionStatus ActionStatus = TranslateTransformActionStatus.Idle;

  private readonly Vector3 _targetPosition;

  private readonly Easing _easing;

  private readonly EasingType _easingType;

  private readonly float _duration;

  private float _startTime;

  private Vector3 _path;

  private Vector3 _startPosition;

  public static TranslateTransformAction Start(
    Vector3 startPosition,
    Vector3 targetPosition,
    float duration,
    EasingType easingType,
    Easing easing)
  {
    var translateTransformAction = new TranslateTransformAction(
      targetPosition,
      duration,
      easingType,
      easing);

    translateTransformAction.Start(startPosition);

    return translateTransformAction;
  }

  public TranslateTransformAction(
    Vector3 targetPosition,
    float duration,
    EasingType easingType,
    Easing easing)
  {
    _easing = easing;
    _easingType = easingType;
    _targetPosition = targetPosition;
    _duration = duration;
  }

  public void Start(Vector3 startPosition)
  {
    ActionStatus = TranslateTransformActionStatus.Started;

    _startTime = Time.time;

    _startPosition = startPosition;

    _path = _targetPosition - _startPosition;
  }

  public Vector3 GetPosition()
  {
    if (ActionStatus == TranslateTransformActionStatus.Completed)
    {
      return _targetPosition;
    }

    if (ActionStatus == TranslateTransformActionStatus.Idle)
    {
      return _startPosition;
    }

    float currentTime = Time.time - _startTime;

    var percentage = _easing.GetValue(_easingType, currentTime, _duration);

    if (percentage >= 1f)
    {
      ActionStatus = TranslateTransformActionStatus.Completed;

      return _targetPosition;
    }

    var translationVector = _startPosition + (_path.normalized * (_path.magnitude * percentage));

    return translationVector;
  }
}
