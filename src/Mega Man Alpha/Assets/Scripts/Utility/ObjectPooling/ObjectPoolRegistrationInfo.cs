using UnityEngine;

public class ObjectPoolRegistrationInfo
{
  public GameObject GameObject;

  public int TotalInstances;

  public ObjectPoolRegistrationInfo(GameObject gameObject, int totalInstances)
  {
    GameObject = gameObject;
    TotalInstances = totalInstances;
  }

  public ObjectPoolRegistrationInfo Clone()
  {
    return new ObjectPoolRegistrationInfo(GameObject, TotalInstances);
  }
}
