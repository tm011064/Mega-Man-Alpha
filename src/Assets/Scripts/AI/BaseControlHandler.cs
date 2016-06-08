using System;
using UnityEngine;

public class BaseControlHandler : IDisposable
{
  protected GameManager GameManager;

  protected float? OverrideEndTime;

  protected CharacterPhysicsManager CharacterPhysicsManager;

  protected Color DebugBoundingBoxColor = Color.green; // TODO (Roman: remove all this from release build

  protected bool DoDrawDebugBoundingBox = false;

  public BaseControlHandler(CharacterPhysicsManager characterPhysicsManager)
    : this(characterPhysicsManager, -1)
  {
  }

  public BaseControlHandler(CharacterPhysicsManager characterPhysicsManager, float duration)
  {
    GameManager = GameManager.Instance;

    CharacterPhysicsManager = characterPhysicsManager;

    OverrideEndTime = duration >= 0f
      ? (float?)(Time.time + duration)
      : null;
  }

  public virtual void DrawGizmos()
  {
  }

  protected virtual void OnAfterUpdate()
  {
  }

  public virtual void OnAfterStackPeekUpdate()
  {
  }

  public virtual void Dispose()
  {
  }

  protected virtual bool DoUpdate()
  {
    return true;
  }

  /// <summary>
  /// This method is called from the BaseCharacterController control handler stack in order to evaluate whether the
  /// top stack element can be activated or not. By default this method always returns true but can be overridden
  /// for special purposes or chained control handlers.
  /// </summary>
  /// <param name="previousControlHandler">The last active control handler.</param>
  /// <returns>true if activation was successful; false if not.</returns>
  public virtual bool TryActivate(BaseControlHandler previousControlHandler)
  {
    Logger.Trace("Activated control handler: " + ToString());

    return true;
  }

  public bool Update()
  {
    if (OverrideEndTime.HasValue && OverrideEndTime < Time.time)
    {
      return false;
    }

    var doUpdate = DoUpdate();

    OnAfterUpdate();

    return doUpdate;
  }
}
