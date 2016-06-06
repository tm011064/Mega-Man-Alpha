﻿public abstract class SingleUpdatePlayerControlHandler : DefaultPlayerControlHandler
{
  private bool _hasCompleted = false;

  public SingleUpdatePlayerControlHandler(PlayerController playerController, float duration)
    : base(playerController, duration)
  {
  }

  protected abstract void OnSingleUpdate();

  protected override bool DoUpdate()
  {
    if (!_hasCompleted)
    {
      OnSingleUpdate();

      _hasCompleted = true;

      return true;
    }

    return false;
  }
}