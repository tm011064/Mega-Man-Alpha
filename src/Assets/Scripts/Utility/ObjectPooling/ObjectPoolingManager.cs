using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager
{
  private static object SyncRoot = new object();

  private static ObjectPoolingManager _instance;

  private Dictionary<string, ObjectPool> _objectPools;

  public event Action<GameObject> BeforeDeactivated;

  public event Action<GameObject> AfterDeactivated;

  private ObjectPoolingManager()
  {
    _objectPools = new Dictionary<String, ObjectPool>();
  }

  public static ObjectPoolingManager Instance
  {
    get
    {
      if (_instance == null)
      {
        lock (SyncRoot)
        {
          if (_instance == null)
          {
            _instance = new ObjectPoolingManager();
          }
        }
      }

      return _instance;
    }
  }

  public bool RegisterPool(GameObject objToPool, int initialPoolSize, int maxPoolSize)
  {
    if (_objectPools.ContainsKey(objToPool.name))
    {
      return false;
    }
    else
    {
      var objectPool = new ObjectPool(objToPool, initialPoolSize, maxPoolSize);

      _objectPools.Add(objToPool.name, objectPool);

      Logger.Info("Registered " + objToPool.name + " at object pool manager");

      return true;
    }
  }

  public GameObject GetObject(string objName, Vector3 position)
  {
    return _objectPools[objName].GetObject(position);
  }

  public GameObject GetObject(string objName)
  {
    Logger.Assert(_objectPools.ContainsKey(objName), "Key " + objName + " not found at object pool");

    return _objectPools[objName].GetObject();
  }

  public void Deactivate(GameObject obj)
  {
    if (obj != null && obj.activeSelf)
    {
      var beforeDeactivatedHandler = BeforeDeactivated;

      if (beforeDeactivatedHandler != null)
      {
        beforeDeactivatedHandler.Invoke(obj);
      }

      obj.SendMessage("OnBeforeDisable", SendMessageOptions.DontRequireReceiver);

      obj.SetActive(false);

      var afterDeactivatedHandler = AfterDeactivated;
      if (afterDeactivatedHandler != null)
      {
        afterDeactivatedHandler.Invoke(obj);
      }
    }
  }

  public void DeactivateAndClearAll()
  {
    DeactivateAll();

    Logger.Info("Clearing object pool.");

    _objectPools = new Dictionary<string, ObjectPool>();
  }

  public void DeactivateAll()
  {
    Logger.Info("Deactivating all pooled objects.");

    foreach (ObjectPool objectPool in _objectPools.Values)
    {
      foreach (GameObject item in objectPool.GetGameObjects())
      {
        if (item.activeSelf)
        {
          Deactivate(item);
        }
      }
    }
  }
}
