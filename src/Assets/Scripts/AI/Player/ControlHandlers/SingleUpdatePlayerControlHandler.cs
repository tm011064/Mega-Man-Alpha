public abstract class SingleUpdatePlayerControlHandler : DefaultPlayerControlHandler
{
  private bool _hasCompleted = false;

  public SingleUpdatePlayerControlHandler(PlayerController playerController, float duration)
    : base(playerController, null, duration)
  {
  }

  protected abstract void OnSingleUpdate();

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (!_hasCompleted)
    {
      OnSingleUpdate();

      _hasCompleted = true;

      return ControlHandlerAfterUpdateStatus.KeepAlive;
    }

    return ControlHandlerAfterUpdateStatus.CanBeDisposed;
  }
}