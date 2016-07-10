using System;
using UnityEngine;

public interface ISpawnable
{
  void Reset();

  event Action<BaseMonoBehaviour> GotDisabled;
}