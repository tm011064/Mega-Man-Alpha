using System;

public interface ISpawnable
{
  bool CanSpawn();

  void Reset();

  event Action<BaseMonoBehaviour> GotDisabled;
}