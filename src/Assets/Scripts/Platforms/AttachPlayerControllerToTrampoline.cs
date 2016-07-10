using System;
using System.Collections.Generic;
using UnityEngine;


public partial class AttachPlayerControllerToTrampoline : MonoBehaviour, IAttachableObject, IObjectPoolBehaviour
{
  public bool CanJump = true;

  public float PlatformDownwardDistance = -32f;

  public float PlatformDownwardDuration = .2f;

  public iTween.EaseType PlatformDownwardEaseType = iTween.EaseType.easeInOutQuart;

  public float PlatformUpwardDuration = .6f;

  public iTween.EaseType PlatformUpwardEaseType = iTween.EaseType.easeOutBounce;

  public float AutoBounceFixedJumpHeight = 224f;

  public float FixedJumpHeight = 448f;

  public float OnTrampolineSkidDamping = 5f;

  private bool _isPlayerControllerAttached = false;

  private bool _isGoingUp = false;

  private bool _hasReachedUpMoveApex = false;

  private bool _hasBounced = false;

  private Vector3 _lastPosition;

  public GameObject trampolinePrefab;

  private GameObject _gameObject;

  protected PlayerController _playerController;

  private TrampolineBounceControlHandler _trampolineBounceControlHandler = null;

  public event Action<IAttachableObject, GameObject> Attached;

  public event Action<IAttachableObject, GameObject> Detached;

  void Awake()
  {
    _playerController = GameManager.Instance.Player;
  }

  void Start()
  {
    _gameObject = ObjectPoolingManager.Instance.GetObject(trampolinePrefab.name);

    _gameObject.transform.position = transform.position;
    _gameObject.transform.parent = transform;

    _lastPosition = _gameObject.transform.position;
  }

  void OnEnable()
  {
    _playerController.GroundedPlatformChanged += OnPlayerGroundedPlatformChanged;
  }

  void OnDisable()
  {
    _playerController.GroundedPlatformChanged -= OnPlayerGroundedPlatformChanged;
  }

  void OnPlayerGroundedPlatformChanged(GroundedPlatformChangedInfo e)
  {
    var lostGround =
      ( // either player is in air
        e.CurrentPlatform == null
        && _playerController.transform.parent == _gameObject.transform
      )
      || e.PreviousPlatform == _gameObject; // or the previous platform was the trampoline 

    if (lostGround)
    {
      // lost ground
      _isPlayerControllerAttached = false;

      _playerController.transform.parent = null;

      if (_trampolineBounceControlHandler != null
        && !_trampolineBounceControlHandler.HasJumped) // we check this in case the player has slid off thr trampoline before being able to jump
      {
        _playerController.RemoveControlHandler(_trampolineBounceControlHandler);
        _trampolineBounceControlHandler = null;
      }

      var handler = Detached;

      if (handler != null)
      {
        handler.Invoke(this, gameObject);
      }

      Logger.Info("Removed parent (" + gameObject.transform + ") relationship from child (" + _playerController.name + ")");
    }
    else if (e.CurrentPlatform == _gameObject)
    {
      if (_playerController.transform.parent != _gameObject.transform)
      {
        _isGoingUp = false;

        _hasBounced = false;

        _hasReachedUpMoveApex = false;

        _isPlayerControllerAttached = true;

        _playerController.transform.parent = _gameObject.transform;

        _trampolineBounceControlHandler = new TrampolineBounceControlHandler(
          _playerController,
          -1f,
          FixedJumpHeight,
          OnTrampolineSkidDamping,
          CanJump);

        _playerController.PushControlHandler(_trampolineBounceControlHandler);

        iTween.MoveBy(
          _gameObject,
          iTween.Hash(
            "y", PlatformDownwardDistance,
            "time", PlatformDownwardDuration,
            "easetype", PlatformDownwardEaseType,
            "oncomplete", "OnDownMoveComplete",
            "oncompletetarget", gameObject));

        var handler = Attached;

        if (handler != null)
        {
          handler.Invoke(this, gameObject);
        }

        Logger.Info("Added parent (" + gameObject.transform + ") relationship to child (" + _playerController.name + ")");
      }
    }
  }

  private void OnUpMoveCompleted()
  {
  }

  private void OnDownMoveComplete()
  {
    _isGoingUp = true;

    iTween.MoveBy(
      _gameObject,
      iTween.Hash(
        "y", -PlatformDownwardDistance,
        "time", PlatformUpwardDuration,
        "easetype", PlatformUpwardEaseType,
        "oncomplete", "OnUpMoveCompleted",
        "oncompletetarget", gameObject));
  }

  void Update()
  {
    if (!_hasReachedUpMoveApex
      && _isGoingUp
      && _lastPosition.y > _gameObject.transform.position.y)
    {
      // we assume there is a bounce out animation, so we want to find out when the up move has reached the apex before bouncing around a bit and getting settled
      _hasReachedUpMoveApex = true;
    }

    if (_isPlayerControllerAttached
      && !_hasBounced
      && _hasReachedUpMoveApex)
    {
      if (_trampolineBounceControlHandler != null)
      {
        _playerController.RemoveControlHandler(_trampolineBounceControlHandler);
        _trampolineBounceControlHandler = null;
      }

      _playerController.PushControlHandler(new TrampolineAutoBounceControlHandler(_playerController, AutoBounceFixedJumpHeight));

      _hasBounced = true;
    }

    _lastPosition = _gameObject.transform.position;
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(trampolinePrefab);
  }
}
