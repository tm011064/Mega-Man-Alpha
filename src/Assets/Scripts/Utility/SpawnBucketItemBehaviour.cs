using System.Collections.Generic;
using UnityEngine;

public class SpawnBucketItemBehaviour : MonoBehaviour
{
  protected IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos(
    GameObject obj,
    int totalInstances = 1)
  {
    yield return new ObjectPoolRegistrationInfo(obj, totalInstances);

    var objectPoolBehaviours = obj.GetComponentsInChildren<IObjectPoolBehaviour>(true);

    if (objectPoolBehaviours != null)
    {
      for (var i = 0; i < objectPoolBehaviours.Length; i++)
      {
        foreach (var registrationInfo in objectPoolBehaviours[i].GetObjectPoolRegistrationInfos())
        {
          yield return registrationInfo;
        }
      }
    }
  }
}
