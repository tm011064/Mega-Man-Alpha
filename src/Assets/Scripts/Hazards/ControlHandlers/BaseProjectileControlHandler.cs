using System;

public abstract class BaseProjectileControlHandler : IDisposable
{
  protected ProjectileController _projectileController;

  public abstract bool Update();

  public virtual void Dispose()
  {
  }

  /// <summary>
  /// This method is called from the BaseCharacterController control handler stack in order to evaluate whether the
  /// top stack element can be activated or not. By default this method always returns true but can be overridden
  /// for special purposes or chained control handlers.
  /// </summary>
  public virtual bool TryActivate(BaseProjectileControlHandler previousControlHandler)
  {
    Logger.Trace("Activated control handler: " + ToString());
    return true;
  }

  public BaseProjectileControlHandler(ProjectileController projectileController)
  {
    _projectileController = projectileController;
  }
}
