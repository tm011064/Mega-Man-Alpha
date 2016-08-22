using System;
using System.Collections;
using UnityEngine;

public class TimedActionEnumerator : IEnumerator
{
  private readonly TimeSpan _delay;

  private readonly Action _callback;

  private bool _isWaiting;

  public TimedActionEnumerator(TimeSpan delay, Action callback)
  {
    _delay = delay;
    _callback = callback;
  }

  public void Reset()
  {
    _isWaiting = false;
  }

  public bool MoveNext()
  {
    if (!_isWaiting)
    {
      return true;
    }

    _callback();

    return false;
  }

  public object Current
  {
    get
    {
      if (!_isWaiting)
      {
        _isWaiting = true;

        return new WaitForSeconds((float)_delay.TotalSeconds);
      }

      return null;
    }
  }
}