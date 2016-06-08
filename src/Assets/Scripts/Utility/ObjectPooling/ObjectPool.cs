using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
  private List<GameObject> _pooledObjects;

  private GameObject _pooledObj;

  private int _maxPoolSize;

  private int _initialPoolSize;

  public ObjectPool(GameObject obj, int initialPoolSize, int maxPoolSize)
  {
    _pooledObjects = new List<GameObject>();

    for (var i = 0; i < initialPoolSize; i++)
    {
      var gameObject = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;

      gameObject.SetActive(false);

      _pooledObjects.Add(gameObject);

      GameObject.DontDestroyOnLoad(gameObject);
    }

    _maxPoolSize = maxPoolSize;

    _pooledObj = obj;

    _initialPoolSize = initialPoolSize;
  }

  /// <summary>
  /// Returns an active object from the object pool without resetting any of its values.
  /// You will need to set its values and set it inactive again when you are done with it.
  /// </summary>
  public GameObject GetObject()
  {
    return GetObject(null);
  }

  /// <summary>
  /// Returns an active object from the object pool without resetting any of its values.
  /// You will need to set its values and set it inactive again when you are done with it.
  /// </summary>
  public GameObject GetObject(Vector3? position)
  {
    for (var i = 0; i < _pooledObjects.Count; i++)
    {
      if (_pooledObjects[i].activeSelf == false)
      {
        if (position.HasValue)
        {
          _pooledObjects[i].transform.position = position.Value;
        }

        _pooledObjects[i].SetActive(true);

        return _pooledObjects[i];
      }
    }

    if (_maxPoolSize > _pooledObjects.Count)
    {
      var gameObject = GameObject.Instantiate(_pooledObj, position.HasValue ? position.Value : Vector3.zero, Quaternion.identity) as GameObject;

      gameObject.SetActive(true);

      _pooledObjects.Add(gameObject);

      return gameObject;
    }

    return null;
  }

  public IEnumerable<GameObject> GetGameObjects()
  {
    for (var i = 0; i < _pooledObjects.Count; i++)
    {
      yield return _pooledObjects[i];
    }
  }

  public void Shrink()
  {
    int objectsToRemoveCount = _pooledObjects.Count - _initialPoolSize;

    if (objectsToRemoveCount <= 0)
    {
      return;
    }

    for (var i = _pooledObjects.Count - 1; i >= 0; i--)
    {
      if (!_pooledObjects[i].activeSelf)
      {
        var gameObject = _pooledObjects[i];

        _pooledObjects.Remove(gameObject);
      }
    }
  }
}
