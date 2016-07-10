using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachObjectToPath : BaseMonoBehaviour, IObjectPoolBehaviour
{
  public GameObject AttachedObject;

  public iTween.EaseType EaseType = iTween.EaseType.easeInOutSine;

  public iTween.LoopType LoopType = iTween.LoopType.pingPong;

  public float Time = 5f;

  public float Delay = 0f;

  public float PauseTimeOnLoopComplete = 0f;

  public float StartPercentage = 0f;

  public MovingPlatformType MovingPlatformType = MovingPlatformType.StartsWhenPlayerLands;

  private GameObject _gameObject;

  private iTweenPath _iTweenPath;

  private bool _isMoving = false;

  private bool _delayExecuted;

  void OnPlayerGroundedPlatformChanged(GroundedPlatformChangedInfo e)
  {
    if (e.CurrentPlatform != _gameObject)
    {
      return; // we need to check that the player landed on this platform
    }

    if (!_isMoving && MovingPlatformType == MovingPlatformType.StartsWhenPlayerLands)
    {
      Logger.Info("Player landed on platform, start move...");

      StartMove();
    }
  }

  private void OnTweenComplete()
  {
    if (PauseTimeOnLoopComplete > 0f)
    {
      StartCoroutine(PauseTween(PauseTimeOnLoopComplete));
    }
  }

  private IEnumerator PauseTween(float waitTime)
  {
    iTween.Pause(_gameObject);

    yield return new WaitForSeconds(waitTime);

    iTween.Resume(_gameObject);
  }

  private void StartMove()
  {
    var adjustedDelay = Delay;

    if (Delay > 0f
      && LoopType != iTween.LoopType.none
      && !_delayExecuted)
    {
      if (!_delayExecuted)
      {
        // since we are looping, the delay would occur on each cycle which is not wanted
        _delayExecuted = true;

        Invoke("StartMove", Delay);

        return;
      }
      else
      {
        adjustedDelay = 0f;
      }
    }

    var path = _iTweenPath.GetPathInWorldSpace();

    _gameObject.transform.position = path[0];

    iTween.MoveTo(
      _gameObject,
      iTween.Hash(
        "path", path,
        "time", Time,
        "easetype", EaseType,
        "looptype", LoopType,
        "delay", adjustedDelay,
        "oncomplete", "OnTweenComplete",
        "oncompletetarget", gameObject));

    _isMoving = true;

    Logger.Info("Start move floating platform " + name);
  }

  void Awake()
  {
    var pathContainer = GetComponent<iTweenPath>();

    pathContainer.SetPathName(Guid.NewGuid().ToString());
  }

  void OnAttached(IAttachableObject attachableObject, GameObject obj)
  {
    if (!_isMoving)
    {
      Logger.Info("Player landed on platform, start move...");

      StartMove();

      attachableObject.Attached -= OnAttached;
    }
  }

  // Use this for initialization
  void Start()
  {

#if UNITY_EDITOR
    if (AttachedObject == null)
    {
      throw new MissingReferenceException("Game object '" + name + "' is missing variable 'attachedObject'.");
    }
#endif

    _gameObject = ObjectPoolingManager.Instance.GetObject(AttachedObject.name);

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
    }

    _iTweenPath = GetComponent<iTweenPath>();

    _gameObject.transform.position = _iTweenPath.GetFirstNodeInWorldSpace();

    _gameObject.SetActive(true);

    if (MovingPlatformType == MovingPlatformType.MovesAlways)
    {
      StartMove();
    }
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(AttachedObject, 5);
  }
}
