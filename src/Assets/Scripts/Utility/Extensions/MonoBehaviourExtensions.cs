using UnityEngine;

public static class MonoBehaviourExtensions
{
  public static GameObject GetChildGameObject(this MonoBehaviour self, string name)
  {
    return self.transform.GetChildGameObject(name);
  }
}
