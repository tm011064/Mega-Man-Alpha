using System;
using System.Collections.Generic;
using UnityEngine;

public partial class DynamicPingPongPath : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public Color OutlineGizmoColor = Color.white;

  public bool ShowGizmoOutline = true;

  public DynamicPingPongPathMotionSettings ForwardMotionSettings = new DynamicPingPongPathMotionSettings();

  public DynamicPingPongPathMotionSettings BackwardMotionSettings = new DynamicPingPongPathMotionSettings();

  public GameObject ObjectToAttach;

  public int NodeCount = 2;

  [HideInInspector]
  public List<Vector3> Nodes = new List<Vector3>() { Vector3.zero, Vector3.zero };

  private bool _isMovingForward = false;

  protected GameObject _gameObject;

  protected float _percentage = 0f;

  private float[] _segmentLengthPercentages = null;

  protected GameManager _gameManager;

  protected virtual void OnForwardMovementCompleted()
  {
  }

  protected virtual void OnBackwardMovementCompleted()
  {
  }

  public Vector3 GetLengthAdjustedPoint(float percentage)
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

    var vector = Nodes[index + 1] - Nodes[index];

    var direction = Nodes[index] + (vector.normalized * (vector.magnitude * percentage));

    return transform.TransformPoint(direction);
  }

  void Update()
  {
    if (_isMovingForward)
    {
      if (_percentage < 1f)
      {
        _percentage = Mathf.Min(1f, _percentage + Time.deltaTime / ForwardMotionSettings.Time);

        _gameObject.transform.position = GetLengthAdjustedPoint(
          _gameManager.Easing.GetValue(ForwardMotionSettings.EasingType, _percentage, 1f));

        if (_percentage == 1f)
        {
          OnForwardMovementCompleted();
        }
      }
    }
    else if (_percentage > 0f)
    {
      _percentage = Mathf.Max(0f, _percentage - Time.deltaTime / BackwardMotionSettings.Time);

      _gameObject.transform.position = GetLengthAdjustedPoint(
        _gameManager.Easing.GetValue(BackwardMotionSettings.EasingType, _percentage, 1f));

      if (_percentage == 0f)
        OnBackwardMovementCompleted();
    }
  }

  public void StartForwardMovement()
  {
    _isMovingForward = true;
  }

  public void StopForwardMovement()
  {
    _isMovingForward = false;
  }

  void OnDisable()
  {
    ObjectPoolingManager.Instance.Deactivate(_gameObject);

    _gameObject = null;

    _percentage = 0f;

    _isMovingForward = false;
  }

  protected virtual void OnEnable()
  {
    if (_gameManager == null)
    {
      _gameManager = GameManager.Instance;
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
    }

    _gameObject = ObjectPoolingManager.Instance.GetObject(ObjectToAttach.name, gameObject.transform.position);

    _percentage = 0f;

    _isMovingForward = false;
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(ObjectToAttach, 1);
  }

  [Serializable]
  public class DynamicPingPongPathMotionSettings
  {
    public EasingType EasingType = EasingType.Linear;

    public float Time;
  }
}
