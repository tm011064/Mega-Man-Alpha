using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class JumpControlledDisappearingPlatformGroup : MonoBehaviour, IObjectPoolBehaviour
{
  public GameObject PlatformPrefab;

  public List<Vector3> PlatformPositions = new List<Vector3>();

  public int TotalVisiblePlatforms = 2;

  public int TotalInitialVisiblePlatforms = 1;

  public JumpControlledDisappearingPlatformMode PlatformMode =
    JumpControlledDisappearingPlatformMode.DisappearWhenLostGround;

  private ObjectPoolingManager _objectPoolingManager;

  private Queue<GameObject> _currentPlatforms = new Queue<GameObject>();

  private int _currentIndex = 0;

  private GameObject _currentPlatform;

  protected PlayerController _playerController;

  private bool _hasLandedOnPlatform = false;

  private bool _isOnPlatform = false;

  private List<Vector3> _worldSpacePlatformCoordinates = new List<Vector3>();

  void OnEnable()
  {
    switch (PlatformMode)
    {
      case JumpControlledDisappearingPlatformMode.DisappearWhenLostGround:
        _playerController.GroundedPlatformChanged += OnDisappearWhenLostGround;
        break;

      case JumpControlledDisappearingPlatformMode.DisappearWhenLandingOnNextPlatform:
        _playerController.GroundedPlatformChanged += OnDisappearWhenLandingOnNextPlatform;
        break;
    }
  }

  void OnDisable()
  {
    switch (PlatformMode)
    {
      case JumpControlledDisappearingPlatformMode.DisappearWhenLostGround:
        _playerController.GroundedPlatformChanged -= OnDisappearWhenLostGround;
        break;

      case JumpControlledDisappearingPlatformMode.DisappearWhenLandingOnNextPlatform:
        _playerController.GroundedPlatformChanged -= OnDisappearWhenLandingOnNextPlatform;
        break;
    }
  }

  void Awake()
  {
    _playerController = GameManager.Instance.Player;
  }

  void Start()
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    if (TotalVisiblePlatforms >= PlatformPositions.Count)
    {
      throw new ArgumentOutOfRangeException("Total visible platforms must be less or equal the number of platform positions.");
    }

    if (TotalInitialVisiblePlatforms >= PlatformPositions.Count)
    {
      throw new ArgumentOutOfRangeException("Total initial visible platforms must be less or equal the number of platform positions.");
    }

    for (var i = 0; i < PlatformPositions.Count; i++)
    {
      _worldSpacePlatformCoordinates.Add(transform.TransformPoint(PlatformPositions[i]));
    }

    for (var i = 0; i < TotalInitialVisiblePlatforms; i++)
    {
      var platform = _objectPoolingManager.GetObject(
        PlatformPrefab.name,
        _worldSpacePlatformCoordinates[i]);

      _currentPlatforms.Enqueue(platform);

      _currentIndex = i;
    }
  }

  void OnDisappearWhenLostGround(GroundedPlatformChangedInfo groundedPlatformChangedInfo)
  {
    var lostGround = groundedPlatformChangedInfo.CurrentPlatform == null;

    if (lostGround)
    {
      if (_hasLandedOnPlatform)
      {
        if (_isOnPlatform)
        {
          if (PlatformMode == JumpControlledDisappearingPlatformMode.DisappearWhenLostGround)
          {
            if (_currentPlatforms.Count >= TotalVisiblePlatforms)
            {
              GameObject platformToRemove = _currentPlatforms.Dequeue();
              StartCoroutine(FadeOutPlatform(platformToRemove, .2f));
            }
          }
        }
      }

      _isOnPlatform = false;

      _currentPlatform = null;
    }
    else
    {
      if (groundedPlatformChangedInfo.CurrentPlatform != _currentPlatform)
      {
        if (_currentPlatforms.Contains(groundedPlatformChangedInfo.CurrentPlatform))
        {
          _currentPlatform = groundedPlatformChangedInfo.CurrentPlatform;

          _hasLandedOnPlatform = true;

          _isOnPlatform = true;

          if (groundedPlatformChangedInfo.CurrentPlatform.transform.position == _worldSpacePlatformCoordinates[_currentIndex])
          {
            // we are on last platform. Make sure we have the correct count

            while (_currentPlatforms.Count >= TotalVisiblePlatforms)
            {
              var platformToRemove = _currentPlatforms.Dequeue();

              StartCoroutine(FadeOutPlatform(platformToRemove, .2f));
            }

            while (_currentPlatforms.Count < TotalVisiblePlatforms)
            {
              _currentIndex++;

              if (_currentIndex >= _worldSpacePlatformCoordinates.Count)
              {
                _currentIndex = 0;
              }

              var platform = _objectPoolingManager.GetObject(
                PlatformPrefab.name,
                _worldSpacePlatformCoordinates[_currentIndex]);

              _currentPlatforms.Enqueue(platform);
            }
          }
        }
        else
        {
          _currentPlatform = null;
        }
      }
    }
  }

  void OnDisappearWhenLandingOnNextPlatform(GroundedPlatformChangedInfo GroundedPlatformChangedInfo)
  {
    if (_currentPlatforms.Contains(GroundedPlatformChangedInfo.CurrentPlatform))
    {
      var isLastPlatform =
        GroundedPlatformChangedInfo.CurrentPlatform.transform.position == _worldSpacePlatformCoordinates[_currentIndex];

      if (isLastPlatform)
      {
        // we are on last platform
        while (_currentPlatforms.Count < TotalVisiblePlatforms + 1)
        {
          _currentIndex++;

          if (_currentIndex >= _worldSpacePlatformCoordinates.Count)
          {
            _currentIndex = 0;
          }

          var platform = _objectPoolingManager.GetObject(
            PlatformPrefab.name,
            _worldSpacePlatformCoordinates[_currentIndex]);

          _currentPlatforms.Enqueue(platform);
        }

        var platformToRemove = _currentPlatforms.Dequeue();

        ObjectPoolingManager.Instance.Deactivate(platformToRemove); // TODO (Roman): notify and run fade animation
      }
    }
  }

  IEnumerator FadeOutPlatform(GameObject platform, float delayTime)
  {
    yield return new WaitForSeconds(delayTime);

    ObjectPoolingManager.Instance.Deactivate(platform); // TODO (Roman): notify and run fade animation
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(PlatformPrefab, TotalVisiblePlatforms);
  }

  public enum JumpControlledDisappearingPlatformMode
  {
    DisappearWhenLandingOnNextPlatform,

    DisappearWhenLostGround
  }
}
