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

  private Func<bool> _testVisibility;

  private float _visibiltyCheckInterval = 0f;

  protected virtual void OnGotVisible()
  {
  }

  protected virtual void OnGotHidden()
  {
  }

  protected virtual void OnEnable()
  {
    var handler = GotEnabled;

    if (handler != null)
    {
      handler(this);
    }
  }

  protected virtual void OnDisable()
  {
    IsVisible = false;

    var handler = GotDisabled;

    if (handler != null)
    {
      handler(this);
    }
  }

  protected bool IsColliderVisible(Collider2D collider)
  {
    return collider.IsVisibleFrom(Camera.main);
  }

  protected void StartVisibilityChecks(float visibiltyCheckInterval, Collider2D collider)
  {
    if (visibiltyCheckInterval > 0f)
    {
      _visibiltyCheckInterval = visibiltyCheckInterval;

      _testVisibility = () => IsColliderVisible(collider);

      StartCoroutine(CheckVisibility());
    }
  }

  protected void StartVisibilityChecks(float visibiltyCheckInterval, Renderer renderer)
  {
    if (visibiltyCheckInterval > 0f)
    {
      _visibiltyCheckInterval = visibiltyCheckInterval;

      _testVisibility = () => renderer.IsVisibleFrom(Camera.main);

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

  /// <summary>
  /// Unity seems to set the bounds extents of a disabled game object's box collider to zero. This method
  /// returns the actual extents of the collider as if it was enabled.
  /// </summary>
  protected Bounds GetBounds(BoxCollider2D collider)
  {
    if (isActiveAndEnabled)
    {
      return collider.bounds;
    }

    var bounds = collider.bounds;

    bounds.size = collider.size;

    return bounds;
  }
}
