using System.Collections.Generic;
using UnityEngine;

public partial class LinearPath : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public Color OutlineGizmoColor = Color.white;

  public bool ShowGizmoOutline = true;

  public EasingType EasingType = EasingType.Linear;

  public GameObject ObjectToAttach;

  public int TotalObjectsOnPath = 1;

  public LoopMode LinerPathLoopMode = LoopMode.Once;

  public bool UseTime = true;

  public bool UseSpeed = false;

  public float LinearPathTime = 5f;

  public float SpeedInUnitsPerSecond = 100;

  public MovingPlatformType MovingPlatformType = MovingPlatformType.StartsWhenPlayerLands;

  public List<GameObject> SynchronizedStartObjects = new List<GameObject>();

  public StartPathDirection LinearPathStartPathDirection = StartPathDirection.Forward;

  public StartPosition LinearPathStartPosition = StartPosition.Center;

  public float StartPositionOffsetPercentage = 0f;

  public float StartDelayOnEnabled = 0f;

  public float DelayBetweenCycles = 0f;

  public bool LookForward;

  public int NodeCount;

  [HideInInspector]
  public List<Vector3> Nodes = new List<Vector3>() { Vector3.zero, Vector3.zero };

  private bool _needsToUnSubscribeAttachedEvent = false;

  private bool _isMoving = false;

  private float _moveStartTime;

  private List<GameObjectTrackingInformation> _gameObjectTrackingInformation = new List<GameObjectTrackingInformation>();

  private IMoveable[] _synchronizedStartObjects;

  private float[] _segmentLengthPercentages = null;

  private GameManager _gameManager;

  private float _totalTime = 0f;

  private void SetStartPositions()
  {
    var step = 0f;

    var itemPositionPercentage = 0f;

    switch (LinearPathStartPosition)
    {
      case StartPosition.Center:

        step = 1f / ((float)TotalObjectsOnPath + 1f);

        itemPositionPercentage = step;

        break;

      case StartPosition.PathStart:

        step = 1f / ((float)TotalObjectsOnPath);

        itemPositionPercentage = 0f;

        break;
    }

    itemPositionPercentage = Mathf.Repeat(itemPositionPercentage + StartPositionOffsetPercentage, 1f);

    for (var i = 0; i < _gameObjectTrackingInformation.Count; i++)
    {
      Vector3 segmentDirectionVector;

      _gameObjectTrackingInformation[i].GameObject.transform.position =
        GetLengthAdjustedPoint(itemPositionPercentage, out segmentDirectionVector);

      _gameObjectTrackingInformation[i].Percentage = itemPositionPercentage;

      if (LookForward)
      {
        var zRotation = Mathf.Atan2(segmentDirectionVector.y, segmentDirectionVector.x) * Mathf.Rad2Deg;

        _gameObjectTrackingInformation[i].GameObject.transform.rotation = Quaternion.Euler(0f, 0f, zRotation - 90);
      }

      itemPositionPercentage = Mathf.Repeat(itemPositionPercentage + step, 1f);
    }
  }

  void OnAttached(IAttachableObject attachableObject, GameObject obj)
  {
    if (!_isMoving)
    {
      Logger.Info("Player landed on platform, start move");

      _isMoving = true;

      _moveStartTime = Time.time + StartDelayOnEnabled;

      for (var i = 0; i < TotalObjectsOnPath; i++)
      {
        _gameObjectTrackingInformation[i].GameObject.GetComponent<IAttachableObject>().Attached -= OnAttached;
      }

      for (var i = 0; i < _synchronizedStartObjects.Length; i++)
      {
        _synchronizedStartObjects[i].StartMove();
      }

      _needsToUnSubscribeAttachedEvent = false;
    }
  }

  void OnPlayerGroundedPlatformChanged(GroundedPlatformChangedInfo e)
  {
    if (!_gameObjectTrackingInformation.Exists(c => c.GameObject == e.CurrentPlatform))
    {
      return;
    }

    if (!_isMoving && MovingPlatformType == MovingPlatformType.StartsWhenPlayerLands)
    {
      Logger.Info("Player landed on platform, start move");

      _isMoving = true;

      _moveStartTime = Time.time + StartDelayOnEnabled;

      for (var i = 0; i < _synchronizedStartObjects.Length; i++)
      {
        _synchronizedStartObjects[i].StartMove();
      }
    }
  }

  public Vector3 GetLengthAdjustedPoint(float percentage, out Vector3 segmentDirectionVector)
  {
    var index = 0;

    if (percentage >= 1f)
    {
      percentage = 1f;

      index = _segmentLengthPercentages.Length - 1;
    }
    else
    {
      var remainingPercentage = percentage;

      for (var i = 0; i < _segmentLengthPercentages.Length; i++)
      {
        if (remainingPercentage > _segmentLengthPercentages[i])
        {
          remainingPercentage -= _segmentLengthPercentages[i];
        }
        else
        {
          percentage = remainingPercentage / _segmentLengthPercentages[i];

          index = i;

          break;
        }
      }
    }

    segmentDirectionVector = Nodes[index + 1] - Nodes[index];

    var direction = Nodes[index] + (segmentDirectionVector.normalized * (segmentDirectionVector.magnitude * percentage));

    return transform.TransformPoint(direction);
  }

  void Update()
  {
    if (_isMoving && Time.time >= _moveStartTime)
    {
      var percentage = Time.deltaTime / _totalTime;

      if (percentage > 0f)
      {
        for (var i = 0; i < _gameObjectTrackingInformation.Count; i++)
        {
          if (Time.time < _gameObjectTrackingInformation[i].NextStartTime)
          {
            // this happens when there is a delay between cycles
            continue;
          }

          _gameObjectTrackingInformation[i].Percentage +=
            (percentage * _gameObjectTrackingInformation[i].DirectionMultiplicationFactor);

          if (_gameObjectTrackingInformation[i].GameObject == null)
          {
            // for loop mode with delay between cycles
            _gameObjectTrackingInformation[i].GameObject = ObjectPoolingManager.Instance.GetObject(ObjectToAttach.name);
          }

          if (_gameObjectTrackingInformation[i].Percentage > 1f)
          {
            // we reached the end, so react

            switch (LinerPathLoopMode)
            {
              case LoopMode.Loop:

                _gameObjectTrackingInformation[i].Percentage = Mathf.Repeat(_gameObjectTrackingInformation[i].Percentage, 1f);

                if (DelayBetweenCycles > 0)
                {
                  ObjectPoolingManager.Instance.Deactivate(_gameObjectTrackingInformation[i].GameObject);

                  _gameObjectTrackingInformation[i].GameObject = null;
                  _gameObjectTrackingInformation[i].NextStartTime = Time.time + DelayBetweenCycles;
                }
                else
                {
                  var newObject = ObjectPoolingManager.Instance.GetObject(ObjectToAttach.name); // it is important to do this before deactivating the existing one to get a new object

                  ObjectPoolingManager.Instance.Deactivate(_gameObjectTrackingInformation[i].GameObject);

                  _gameObjectTrackingInformation[i].GameObject = newObject;
                }

                break;

              case LoopMode.Once:

                _gameObjectTrackingInformation[i].Percentage = 1f;

                break;

              case LoopMode.PingPong:

                _gameObjectTrackingInformation[i].Percentage = 1f - Mathf.Repeat(_gameObjectTrackingInformation[i].Percentage, 1f);
                _gameObjectTrackingInformation[i].DirectionMultiplicationFactor *= -1f;
                _gameObjectTrackingInformation[i].NextStartTime = Time.time + DelayBetweenCycles;

                break;
            }
          }
          else if (_gameObjectTrackingInformation[i].Percentage < 0f)
          {
            switch (LinerPathLoopMode)
            {
              case LoopMode.Loop:

                _gameObjectTrackingInformation[i].Percentage =
                  1f - Mathf.Repeat(_gameObjectTrackingInformation[i].Percentage * -1f, 1f);

                if (DelayBetweenCycles > 0)
                {
                  ObjectPoolingManager.Instance.Deactivate(_gameObjectTrackingInformation[i].GameObject);

                  _gameObjectTrackingInformation[i].GameObject = null;
                  _gameObjectTrackingInformation[i].NextStartTime = Time.time + DelayBetweenCycles;
                }
                else
                {
                  var newObject = ObjectPoolingManager.Instance.GetObject(ObjectToAttach.name); // it is important to do this before deactivating the existing one to get a new object

                  ObjectPoolingManager.Instance.Deactivate(_gameObjectTrackingInformation[i].GameObject);

                  _gameObjectTrackingInformation[i].GameObject = newObject;
                }

                break;

              case LoopMode.Once:

                _gameObjectTrackingInformation[i].Percentage = 0f;

                break;

              case LoopMode.PingPong:

                _gameObjectTrackingInformation[i].Percentage = Mathf.Repeat(_gameObjectTrackingInformation[i].Percentage * -1f, 1f);
                _gameObjectTrackingInformation[i].DirectionMultiplicationFactor *= -1f;
                _gameObjectTrackingInformation[i].NextStartTime = Time.time + DelayBetweenCycles;

                break;
            }
          }

          if (_gameObjectTrackingInformation[i].GameObject != null)
          {
            // can be null if loop mode with delay

            Vector3 segmentDirectionVector;

            _gameObjectTrackingInformation[i].GameObject.transform.position = GetLengthAdjustedPoint(
              _gameManager.Easing.GetValue(EasingType, _gameObjectTrackingInformation[i].Percentage, 1f),
              out segmentDirectionVector);

            if (LookForward)
            {
              float zRotation = Mathf.Atan2(segmentDirectionVector.y, segmentDirectionVector.x) * Mathf.Rad2Deg;

              _gameObjectTrackingInformation[i].GameObject.transform.rotation = Quaternion.Euler(0f, 0f, zRotation - 90);
            }
          }
        }
      }
    }
  }

  void OnDisable()
  {
    if (_needsToUnSubscribeAttachedEvent)
    {
      for (var i = 0; i < TotalObjectsOnPath; i++)
      {
        _gameObjectTrackingInformation[i].GameObject.GetComponent<IAttachableObject>().Attached -= OnAttached;
      }

      _needsToUnSubscribeAttachedEvent = false;
    }

    for (var i = _gameObjectTrackingInformation.Count - 1; i >= 0; i--)
    {
      ObjectPoolingManager.Instance.Deactivate(_gameObjectTrackingInformation[i].GameObject);

      _gameObjectTrackingInformation[i].GameObject = null;
    }

    _gameObjectTrackingInformation = new List<GameObjectTrackingInformation>();

    _isMoving = false;

    Logger.Info("Disabled moving linear path " + name);
  }

  void OnEnable()
  {
    Logger.Info("Enabled moving linear path " + name);

    if (_gameManager == null)
    {
      _gameManager = GameManager.Instance;
    }

    if (_synchronizedStartObjects == null)
    {
      var moveables = new List<IMoveable>(SynchronizedStartObjects.Count);

      for (var i = 0; i < SynchronizedStartObjects.Count; i++)
      {
        var moveable = SynchronizedStartObjects[i].GetComponent<IMoveable>();

        if (moveable != null)
        {
          moveables.Add(moveable);
        }
      }

      _synchronizedStartObjects = moveables.ToArray();
    }

    if (_segmentLengthPercentages == null)
    {
      // first calculate lengths      
      var totalLength = 0f;

      var segmentLengths = new float[Nodes.Count - 1];

      for (var i = 1; i < Nodes.Count; i++)
      {
        var distance = Vector3.Distance(Nodes[i - 1], Nodes[i]);

        segmentLengths[i - 1] = distance;

        totalLength += distance;
      }

      var segmentLengthPercentages = new float[segmentLengths.Length];

      for (var i = 0; i < segmentLengths.Length; i++)
      {
        segmentLengthPercentages[i] = segmentLengths[i] / totalLength;
      }

      _segmentLengthPercentages = segmentLengthPercentages;

      if (UseTime)
      {
        _totalTime = LinearPathTime;
      }
      else if (UseSpeed)
      {
        Logger.Assert(SpeedInUnitsPerSecond > 0f, "Speed must be greater than 0.");

        _totalTime = totalLength / SpeedInUnitsPerSecond;
      }

      Logger.Assert(_totalTime > 0f, "Time must be set to a positive value greater than 0");
    }

    _gameObjectTrackingInformation = new List<GameObjectTrackingInformation>();

    for (var i = 0; i < TotalObjectsOnPath; i++)
    {
      _gameObjectTrackingInformation.Add(
        new GameObjectTrackingInformation(
          ObjectPoolingManager.Instance.GetObject(ObjectToAttach.name),
          0f,
          LinearPathStartPathDirection == StartPathDirection.Forward
            ? 1f
            : -1f));
    }

    if (MovingPlatformType == MovingPlatformType.StartsWhenPlayerLands)
    {
      for (var i = 0; i < TotalObjectsOnPath; i++)
      {
        var attachableObject = _gameObjectTrackingInformation[i].GameObject.GetComponent<IAttachableObject>();

        if (attachableObject != null)
        {
          attachableObject.Attached += OnAttached;

          _needsToUnSubscribeAttachedEvent = true;
        }
        else
        {
          GameManager.Instance.Player.GroundedPlatformChanged += OnPlayerGroundedPlatformChanged;
        }
      }
    }

    SetStartPositions();

    if (MovingPlatformType == MovingPlatformType.MovesAlways)
    {
      _isMoving = true;

      _moveStartTime = Time.time + StartDelayOnEnabled;

      Logger.Info("Start moving linear path " + name);
    }
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    return GetObjectPoolRegistrationInfos(ObjectToAttach, TotalObjectsOnPath);
  }

  public enum StartPosition
  {
    PathStart,

    Center
  }

  public enum LoopMode
  {
    Once,

    Loop,

    PingPong
  }

  public enum StartPathDirection
  {
    Forward,

    Backward
  }

  private class GameObjectTrackingInformation
  {
    public GameObject GameObject;

    public float Percentage = 0f;

    public float DirectionMultiplicationFactor = 1f;

    public float NextStartTime = 0f;

    public GameObjectTrackingInformation(GameObject gameObject, float percentage, float directionMultiplicationFactor)
    {
      GameObject = gameObject;
      Percentage = percentage;
      DirectionMultiplicationFactor = directionMultiplicationFactor;
    }
  }
}
