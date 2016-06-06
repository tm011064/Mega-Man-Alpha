using System;
using UnityEngine;

public interface IAttachableObject
{
  event Action<IAttachableObject, GameObject> Attached;

  event Action<IAttachableObject, GameObject> Detached;
}
