using System;
using System.Collections;
using UnityEngine;

public class BaseMonoBehaviour : MonoBehaviour
{
  public event Action<BaseMonoBehaviour> GotDisabled;

  public event Action<BaseMonoBehaviour> GotEnabled;

  public event Action<BaseMonoBehaviour> GotVisible;

  public event Action<BaseMonoBehaviour> GotHidden;

  protected bool IsVisible;

  private Collider2D _visibilityCheckCollider;

  private Renderer _visibilityCheckRenderer;

  private Func<bool> _testVisibility;

  private float _visibiltyCheckInterval = 0f;

  protected virtual void OnGotVisible()
  {
  }

  protected virtual void OnGotHidden()
  {
  }

  void OnEnable()
  {
    var handler = GotEnabled;

    if (handler != null)
    {
      handler(this);
    }
  }

  void OnDisable()
  {
    IsVisible = false;

    var handler = GotDisabled;

    if (handler != null)
    {
      handler(this);
    }
  }

  protected void StartVisibilityChecks(float visibiltyCheckInterval, Collider2D collider)
  {
    if (visibiltyCheckInterval > 0f)
    {
      _visibiltyCheckInterval = visibiltyCheckInterval;

      _visibilityCheckCollider = collider;

      _testVisibility = (() => { return _visibilityCheckCollider.IsVisibleFrom(Camera.main); });

      StartCoroutine(CheckVisibility());
    }
  }

  protected void StartVisibilityChecks(float visibiltyCheckInterval, Renderer renderer)
  {
    if (visibiltyCheckInterval > 0f)
    {
      _visibiltyCheckInterval = visibiltyCheckInterval;

      _visibilityCheckRenderer = renderer;

      _testVisibility = (() => { return _visibilityCheckRenderer.IsVisibleFrom(Camera.main); });

      StartCoroutine(CheckVisibility());
    }
  }

  IEnumerator CheckVisibility()
  {
    while (true)
    {
      var isVisible = _testVisibility();

      if (isVisible && !IsVisible)
      {
        OnGotVisible();

        var handler = GotVisible;

        if (handler != null)
        {
          handler(this);
        }
      }
      else if (!isVisible && IsVisible)
      {
        OnGotHidden();

        var handler = GotHidden;

        if (handler != null)
        {
          handler(this);
        }
      }

      IsVisible = isVisible;

      yield return new WaitForSeconds(_visibiltyCheckInterval);
    }
  }
}
