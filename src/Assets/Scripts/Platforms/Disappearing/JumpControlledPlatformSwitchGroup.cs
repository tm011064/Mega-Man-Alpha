using System;
using System.Collections.Generic;
using UnityEngine;

public partial class JumpControlledPlatformSwitchGroup : SpawnBucketItemBehaviour, IObjectPoolBehaviour
{
  public List<JumpSwitchGroup> PlatformGroupPositions = new List<JumpSwitchGroup>();

  public int PlatformGroupStartIndex = 0;

  private ObjectPoolingManager _objectPoolingManager;

  private int _currentEnabledGroupIndex = 0;

  protected PlayerController _playerController;

  void OnEnable()
  {
    _playerController = GameManager.Instance.Player;

    if (_objectPoolingManager == null)
    {
      _objectPoolingManager = ObjectPoolingManager.Instance;

      for (var i = 0; i < PlatformGroupPositions.Count; i++)
      {
        PlatformGroupPositions[i].GameObjects = new GameObject[PlatformGroupPositions[i].Positions.Count];

        PlatformGroupPositions[i].WorldSpaceCoordinates = new Vector3[PlatformGroupPositions[i].Positions.Count];

        for (int j = 0; j < PlatformGroupPositions[i].Positions.Count; j++)
        {
          PlatformGroupPositions[i].WorldSpaceCoordinates[j] =
            transform.TransformPoint(PlatformGroupPositions[i].Positions[j]);
        }
      }
    }

    if (PlatformGroupPositions.Count < 2)
    {
      throw new ArgumentOutOfRangeException("There must be at least two platform position groups.");
    }

    if (PlatformGroupStartIndex < 0 || PlatformGroupStartIndex >= PlatformGroupPositions.Count)
    {
      SwitchGroups(0);
    }
    else
    {
      SwitchGroups(PlatformGroupStartIndex);
    }

    _playerController.JumpedThisFrame += OnPlayerJumpedThisFrame;
  }

  void OnDisable()
  {
    for (var i = 0; i < PlatformGroupPositions.Count; i++)
    {
      for (int j = 0; j < PlatformGroupPositions[i].WorldSpaceCoordinates.Length; j++)
      {
        if (PlatformGroupPositions[i].GameObjects[j] != null)
        {
          _objectPoolingManager.Deactivate(PlatformGroupPositions[i].GameObjects[j]);

          PlatformGroupPositions[i].GameObjects[j] = null;
        }
      }
    }

    _playerController.JumpedThisFrame -= OnPlayerJumpedThisFrame;
  }

  private void SwitchGroups(int enabledIndex)
  {
    _currentEnabledGroupIndex = enabledIndex;

    for (var i = 0; i < PlatformGroupPositions.Count; i++)
    {
      for (int j = 0; j < PlatformGroupPositions[i].WorldSpaceCoordinates.Length; j++)
      {
        if (PlatformGroupPositions[i].GameObjects[j] != null)
        {
          _objectPoolingManager.Deactivate(PlatformGroupPositions[i].GameObjects[j]);

          PlatformGroupPositions[i].GameObjects[j] = null;
        }

        var name = _currentEnabledGroupIndex == i
            ? PlatformGroupPositions[i].EnabledGameObject.name
            : PlatformGroupPositions[i].DisabledGameObject.name;

        PlatformGroupPositions[i].GameObjects[j] = _objectPoolingManager.GetObject(
          name,
          PlatformGroupPositions[i].WorldSpaceCoordinates[j]);
      }
    }
  }

  void OnPlayerJumpedThisFrame()
  {
    var groupIndex = _currentEnabledGroupIndex + 1 >= PlatformGroupPositions.Count
      ? 0
      : _currentEnabledGroupIndex + 1;

    SwitchGroups(groupIndex);
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    var count = PlatformGroupPositions.Count * 2;

    var objectPoolRegistrationInfos = new ObjectPoolRegistrationInfo[count];

    var index = 0;

    while (index < count)
    {
      objectPoolRegistrationInfos[index] = new ObjectPoolRegistrationInfo(
        PlatformGroupPositions[index].EnabledGameObject,
        PlatformGroupPositions[index].Positions.Count);

      index++;

      objectPoolRegistrationInfos[index] = new ObjectPoolRegistrationInfo(
        PlatformGroupPositions[index].DisabledGameObject,
        PlatformGroupPositions[index].Positions.Count);

      index++;
    }

    return objectPoolRegistrationInfos;
  }

  [Serializable]
  public class JumpSwitchGroup
  {
#if UNITY_EDITOR
    public Color OutlineGizmoColor = Color.yellow;

    public bool ShowGizmoOutline = true;
#endif

    public GameObject EnabledGameObject;

    public GameObject DisabledGameObject;

    public List<Vector3> Positions = new List<Vector3>();

    [HideInInspector]
    public Vector3[] WorldSpaceCoordinates;

    [HideInInspector]
    public GameObject[] GameObjects;
  }
}