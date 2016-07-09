using System;
using UnityEngine;

public interface ISpawnable
{
  void Reset(Vector3 scale);

  event Action<BaseMonoBehaviour> GotDisabled;
}