using System;
using System.Collections.Generic;
using UnityEngine;

public partial class JumpControlledPlatformSwitchGroupWheel : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public int TotalPlatforms = 4;

  public float Width = 256f;

  public float Height = 128f;

  public float Speed = 35f;

  public List<JumpSwitchGroup> PlatformGroups = new List<JumpSwitchGroup>();

  public int PlatformGroupStartIndex = 0;

  private ObjectPoolingManager _objectPoolingManager;

  private int _currentEnabledGroupIndex = 0;

  protected PlayerController _playerController;

  void OnEnable()
  {
    if (PlatformGroups.Count < 1)
    {
      throw new ArgumentOutOfRangeException("There must be at least two platform position groups.");
    }

    _playerController = GameManager.Instance.Player;

    _objectPoolingManager = ObjectPoolingManager.Instance;

    for (var i = 0; i < PlatformGroups.Count; i++)
    {
      PlatformGroups[i].GameObjects = new List<GameObjectContainer>();
    }

    var index = 0;

    var twoPi = Mathf.PI * 2f;

    for (var angle = 0f; angle < twoPi; angle += twoPi / TotalPlatforms)
    {
      var quaternion = Quaternion.AngleAxis(angle, Vector3.forward);

      var rotated = new Vector3(Width * Mathf.Cos(angle), Height * Mathf.Sin(angle), 0.0f);

      rotated = quaternion * rotated + transform.position;

      GameObject platform = null; // note: we do allow null game objects in case it is a wheel with a single platform switch group

      if (PlatformGroups[index].DisabledGameObject != null)
      {
        platform = _objectPoolingManager.GetObject(PlatformGroups[index].DisabledGameObject.name);

        platform.transform.position = rotated;
      }

      PlatformGroups[index].GameObjects.Add(
        new GameObjectContainer() { GameObject = platform, Angle = angle });

      index = index < PlatformGroups.Count - 1
        ? index + 1
        : 0;
    }

    if (PlatformGroupStartIndex < 0 || PlatformGroupStartIndex >= PlatformGroups.Count)
    {
      SwitchGroups(0);
    }
    else
    {
      SwitchGroups(PlatformGroupStartIndex);
    }

    _playerController.JumpedThisFrame += OnPlayerJumpedThisFrame;
  }

  void Update()
  {
    var angleToRotate = Speed * Mathf.Deg2Rad * Time.deltaTime;

    for (var i = 0; i < PlatformGroups.Count; i++)
    {
      for (int j = 0; j < PlatformGroups[i].GameObjects.Count; j++)
      {
        if (PlatformGroups[i].GameObjects[j].GameObject != null)
        {
          PlatformGroups[i].GameObjects[j].Angle += angleToRotate;

          var quaternion = Quaternion.AngleAxis(PlatformGroups[i].GameObjects[j].Angle, Vector3.forward);

          var rotated = new Vector3(
            Width * Mathf.Cos(PlatformGroups[i].GameObjects[j].Angle),
            Height * Mathf.Sin(PlatformGroups[i].GameObjects[j].Angle),
            0.0f);

          rotated = quaternion * rotated + transform.position;

          PlatformGroups[i].GameObjects[j].GameObject.transform.position = rotated;
        }
      }
    }
  }

  void OnDisable()
  {
    for (var i = 0; i < PlatformGroups.Count; i++)
    {
      for (int j = 0; j < PlatformGroups[i].GameObjects.Count; j++)
      {
        if (PlatformGroups[i].GameObjects[j].GameObject != null)
        {
          _objectPoolingManager.Deactivate(PlatformGroups[i].GameObjects[j].GameObject);
        }

        PlatformGroups[i].GameObjects[j] = null;
      }
    }

    _playerController.JumpedThisFrame -= OnPlayerJumpedThisFrame;
  }

  private void SwitchGroups(int enabledIndex)
  {
    _currentEnabledGroupIndex = enabledIndex;

    for (var i = 0; i < PlatformGroups.Count; i++)
    {
      for (int j = 0; j < PlatformGroups[i].GameObjects.Count; j++)
      {
        if (PlatformGroups[i].GameObjects[j].GameObject != null)
        {
          var position = PlatformGroups[i].GameObjects[j].GameObject.transform.position;

          _objectPoolingManager.Deactivate(PlatformGroups[i].GameObjects[j].GameObject);

          var name = _currentEnabledGroupIndex == i
              ? PlatformGroups[i].EnabledGameObject.name
              : PlatformGroups[i].DisabledGameObject.name;

          PlatformGroups[i].GameObjects[j].GameObject = _objectPoolingManager.GetObject(
            name,
            position);
        }
      }
    }
  }

  void OnPlayerJumpedThisFrame()
  {
    var groupIndex = _currentEnabledGroupIndex + 1 >= PlatformGroups.Count
      ? 0
      : _currentEnabledGroupIndex + 1;

    SwitchGroups(groupIndex);
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    var objectPoolRegistrationInfos = new List<ObjectPoolRegistrationInfo>();

    for (var i = 0; i < PlatformGroups.Count; i++)
    {
      if (PlatformGroups[i].EnabledGameObject != null)
      {
        objectPoolRegistrationInfos.Add(new ObjectPoolRegistrationInfo(
          PlatformGroups[i].EnabledGameObject,
          ((TotalPlatforms + 1) / PlatformGroups.Count)));
      }

      if (PlatformGroups[i].DisabledGameObject != null)
      {
        objectPoolRegistrationInfos.Add(new ObjectPoolRegistrationInfo(
          PlatformGroups[i].DisabledGameObject,
          ((TotalPlatforms + 1) / PlatformGroups.Count)));
      }
    }

    return objectPoolRegistrationInfos;
  }

  public class GameObjectContainer
  {
    public GameObject GameObject;

    public float Angle;
  }

  [Serializable]
  public class JumpSwitchGroup
  {
    public GameObject EnabledGameObject;

    public GameObject DisabledGameObject;

    [HideInInspector]
    public List<GameObjectContainer> GameObjects;
  }
}
