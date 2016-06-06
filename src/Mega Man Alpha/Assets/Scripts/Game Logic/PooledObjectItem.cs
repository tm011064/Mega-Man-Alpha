using System;
using UnityEngine;

[Serializable]
public class PooledObjectItem
{
  public GameObject Prefab;

  public int InitialSize = 1;
}
