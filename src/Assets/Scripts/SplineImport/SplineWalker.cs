using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : MonoBehaviour, IObjectPoolBehaviour
{
  public BezierSpline Spline;

  public float Duration;

  public bool LookForward;

  public SplineWalkerMode Mode;

  public MovingPlatformType MovingPlatformType = MovingPlatformType.StartsWhenPlayerLands;

  public GameObject MovingObjectPrefab;

  public bool GoingForward = true;

  public float StartPositionAsPercentage = 0f;

  private float _progress;

  private GameObject _gameObject;

  private bool _isMoving = false;

  void OnPlayerGroundedPlatformChanged(GroundedPlatformChangedInfo e)
  {
    if (e.CurrentPlatform != _gameObject)
    {
      return; // we need to check that the player landed on this platform
    }

    if (!_isMoving && MovingPlatformType == MovingPlatformType.StartsWhenPlayerLands)
    {
      Logger.Info("Player landed on platform, start move...");

      _isMoving = true;
    }
  }

  void OnAttached(IAttachableObject attachableObject, GameObject obj)
  {
    if (!_isMoving)
    {
      Logger.Info("Player landed on platform, start move...");

      _isMoving = true;

      attachableObject.Attached -= OnAttached;
    }
  }

  void Start()
  {
    _gameObject = ObjectPoolingManager.Instance.GetObject(MovingObjectPrefab.name);

    if (!GoingForward)
    {
      _progress = 1f - Mathf.Clamp01(StartPositionAsPercentage);
    }
    else
    {
      _progress = Mathf.Clamp01(StartPositionAsPercentage);
    }

    Spline.CalculateLengths(10);

    if (MovingPlatformType == MovingPlatformType.StartsWhenPlayerLands)
    {
      var attachableObject = _gameObject.GetComponent<IAttachableObject>();

      if (attachableObject != null)
      {
        attachableObject.Attached += OnAttached;
      }
      else
      {
        GameManager.Instance.Player.GroundedPlatformChanged += OnPlayerGroundedPlatformChanged;
      }

      _gameObject.transform.localPosition = Spline.GetLengthAdjustedPoint(_progress);
    }
    else
    {
      _isMoving = true;
    }
  }

  private void Update()
  {
    if (!_isMoving)
    {
      return;
    }

    if (GoingForward)
    {
      _progress += Time.deltaTime / Duration;

      if (_progress > 1f)
      {
        if (Mode == SplineWalkerMode.Once)
        {
          _progress = 1f;
        }
        else if (Mode == SplineWalkerMode.Loop)
        {
          _progress -= 1f;
        }
        else
        {
          _progress = 2f - _progress;

          GoingForward = false;
        }
      }
    }
    else
    {
      _progress -= Time.deltaTime / Duration;

      if (_progress < 0f)
      {
        if (Mode == SplineWalkerMode.Once)
        {
          _progress = 1f;
        }
        else if (Mode == SplineWalkerMode.Loop)
        {
          _progress = 1f - _progress;
        }
        else
        {
          _progress = -_progress;

          GoingForward = true;
        }
      }
    }

    _gameObject.transform.localPosition = Spline.GetLengthAdjustedPoint(_progress);

    if (LookForward)
    {
      var direction = Spline.GetLengthAdjustedDirection(_progress);

      var zRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

      _gameObject.transform.rotation = Quaternion.Euler(0f, 0f, zRotation - 90);
    }
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(MovingObjectPrefab);
  }
}
