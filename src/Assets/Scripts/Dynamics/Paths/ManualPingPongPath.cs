public class ManualPingPongPath : DynamicPingPongPath, IMoveable
{
  public float PauseTimeAfterForwardMoveCompleted = 0f;

  public float PauseTimeAfterBackwardMoveCompleted = 0f;

  public bool AutoStart = true;

  protected override void OnForwardMovementCompleted()
  {
    if (PauseTimeAfterForwardMoveCompleted > 0f)
    {
      Invoke("StopForwardMovement", PauseTimeAfterForwardMoveCompleted);
    }
    else
    {
      StopForwardMovement();
    }
  }

  protected override void OnBackwardMovementCompleted()
  {
    if (PauseTimeAfterBackwardMoveCompleted > 0f)
    {
      Invoke("StartForwardMovement", PauseTimeAfterBackwardMoveCompleted);
    }
    else
    {
      StartForwardMovement();
    }
  }

  protected override void OnEnable()
  {
    base.OnEnable();

    if (AutoStart)
    {
      StartForwardMovement();
    }
  }

  public void StartMove()
  {
    StartForwardMovement();
  }

  public void StopMove()
  {
    StopForwardMovement();
  }
}
