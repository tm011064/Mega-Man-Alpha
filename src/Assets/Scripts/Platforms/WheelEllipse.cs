using System.Collections.Generic;
using UnityEngine;

public partial class WheelEllipse : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public GameObject FloatingAttachedPlatform;

  public float TotalPlatforms = 4;

  public float Width = 256f;

  public float Height = 128f;

  public float Speed = 35f;

  private List<GameObjectContainer> _platforms = new List<GameObjectContainer>();

  private bool _isPlayerAttached;

  private BoxCollider2D _visibilityCollider;

  private ObjectPoolingManager _objectPoolingManager;

  void Update()
  {
    if (_platforms.Count > 0)
    {
      var angleToRotate = Speed * Mathf.Deg2Rad * Time.deltaTime;

      for (var i = 0; i < _platforms.Count; i++)
      {
        _platforms[i].Angle += angleToRotate;

        var quaternion = Quaternion.AngleAxis(_platforms[i].Angle, Vector3.forward);

        var rotated = new Vector3(
          Width * Mathf.Cos(_platforms[i].Angle),
          Height * Mathf.Sin(_platforms[i].Angle),
          0.0f);

        rotated = quaternion * rotated + transform.position;

        _platforms[i].GameObject.transform.position = rotated;
      }
    }
  }

  void OnEnable()
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    var platforms = new List<GameObjectContainer>();

    var twoPi = Mathf.PI * 2f;

    for (var angle = 0f; angle < twoPi; angle += twoPi / TotalPlatforms)
    {
      var platform = _objectPoolingManager.GetObject(FloatingAttachedPlatform.name);

      var quaternion = Quaternion.AngleAxis(angle, Vector3.forward);

      var rotated = new Vector3(
        Width * Mathf.Cos(angle),
        Height * Mathf.Sin(angle),
        0.0f);

      rotated = quaternion * rotated + transform.position;

      platform.transform.position = rotated;

      platforms.Add(new GameObjectContainer { GameObject = platform, Angle = angle });
    }

    _platforms = platforms;
  }

  void OnDisable()
  {
    for (var i = _platforms.Count - 1; i >= 0; i--)
    {
      _objectPoolingManager.Deactivate(_platforms[i].GameObject);
    }

    _platforms.Clear();
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(FloatingAttachedPlatform, (int)TotalPlatforms);
  }

  private class GameObjectContainer
  {
    public GameObject GameObject;

    public float Angle;
  }
}
