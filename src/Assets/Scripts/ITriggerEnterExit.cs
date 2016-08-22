using System;

public interface ITriggerEnterExit
{
  event EventHandler<TriggerEnterExitEventArgs> Entered;

  event EventHandler<TriggerEnterExitEventArgs> Exited;
}
