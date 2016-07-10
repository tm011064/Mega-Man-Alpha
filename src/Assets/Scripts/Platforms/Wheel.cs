using System.Collections.Generic;
using UnityEngine;

public partial class Wheel : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public GameObject FloatingAttachedPlatform;

  public float TotalPlatforms = 3;

  public float Radius = 256f;

  public float Speed = 200f;

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

        var initial = new Vector3(transform.position.x + Radius, transform.position.y, transform.position.z);

        var rotated = new Vector3(
          Mathf.Cos(_platforms[i].Angle) * (initial.x - transform.position.x) - Mathf.Sin(_platforms[i].Angle) * (initial.y - transform.position.y) + transform.position.x,
          Mathf.Sin(_platforms[i].Angle) * (initial.x - transform.position.x) + Mathf.Cos(_platforms[i].Angle) * (initial.y - transform.position.y) + transform.position.y,
          transform.position.z);

        _platforms[i].GameObject.transform.position = rotated;
      }
    }
  }

  void OnEnable()
  {
    _objectPoolingManager = ObjectPoolingManager.Instance;

    Logger.Info("Enabling wheel " + name);

    var platforms = new List<GameObjectContainer>();

    for (var angle = 0f; angle < 360 * Mathf.Deg2Rad; angle += 360 * Mathf.Deg2Rad / TotalPlatforms)
    {
      var platform = _objectPoolingManager.GetObject(FloatingAttachedPlatform.name);

      var initial = new Vector3(transform.position.x + Radius, transform.position.y, transform.position.z);

      var rotated = new Vector3(
        Mathf.Cos(angle) * (initial.x - transform.position.x) - Mathf.Sin(angle) * (initial.y - transform.position.y) + transform.position.x,
        Mathf.Sin(angle) * (initial.x - transform.position.x) - Mathf.Cos(angle) * (initial.y - transform.position.y) + transform.position.y,
        transform.position.z);

      platform.transform.position = rotated;

      platforms.Add(new GameObjectContainer { GameObject = platform, Angle = angle });
    }

    _platforms = platforms;
  }

  void OnDisable()
  {
    Logger.Info("Disabling wheel " + name);

    for (var i = _platforms.Count - 1; i >= 0; i--)
    {
      _objectPoolingManager.Deactivate(_platforms[i].GameObject);
    }

    _platforms.Clear();
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    return GetObjectPoolRegistrationInfos(FloatingAttachedPlatform, (int)TotalPlatforms);
  }

  private class GameObjectContainer
  {
    public GameObject GameObject;

    public float Angle;
  }
}
