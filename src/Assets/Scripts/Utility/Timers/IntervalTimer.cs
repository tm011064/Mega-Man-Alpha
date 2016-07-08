using UnityEngine;

public abstract class IntervalTimer
{
  private readonly float _interval;

  private float _nextCallbackTime;

  public IntervalTimer(float interval)
  {
    _interval = interval;

    Reset();
  }

  public abstract void Callback();

  public void Reset()
  {
    _nextCallbackTime = Time.time + _interval;
  }

  public void Update()
  {
    if (Time.time < _nextCallbackTime)
    {
      return;
    }

    _nextCallbackTime += _interval;

    if (_nextCallbackTime <= Time.time)
    {
      var delta = Time.time - _nextCallbackTime;

      _nextCallbackTime += (Mathf.Floor(delta / _nextCallbackTime) + 1) * _interval;
    }

    Callback();
  }
}
