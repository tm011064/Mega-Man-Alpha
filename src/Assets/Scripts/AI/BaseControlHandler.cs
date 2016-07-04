using System;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Control handler's define how a character reacts to user input in respect to the character's
/// current state and environment.
/// </summary>
public class BaseControlHandler : IDisposable
{
  protected GameManager GameManager;

  protected float? OverrideEndTime;

  protected float Duration;

  protected CharacterPhysicsManager CharacterPhysicsManager;

  protected Color DebugBoundingBoxColor = Color.green;

  protected bool DoDrawDebugBoundingBox = false;

  public BaseControlHandler(CharacterPhysicsManager characterPhysicsManager)
    : this(characterPhysicsManager, -1)
  {
  }

  public BaseControlHandler(CharacterPhysicsManager characterPhysicsManager, float duration)
  {
    GameManager = GameManager.Instance;

    CharacterPhysicsManager = characterPhysicsManager;

    Duration = duration;

    ResetOverrideEndTime();
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

  public virtual BaseControlHandler Clone()
  {
    return MemberwiseClone() as BaseControlHandler;
  }

  protected void ResetOverrideEndTime()
  {
    OverrideEndTime = Duration >= 0f
      ? (float?)(Time.time + Duration)
      : null;
  }

  protected virtual ControlHandlerAfterUpdateStatus DoUpdate()
  {
    return ControlHandlerAfterUpdateStatus.KeepAlive;
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

  public ControlHandlerAfterUpdateStatus Update()
  {
    if (OverrideEndTime.HasValue && OverrideEndTime < Time.time)
    {
      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var doUpdate = DoUpdate();

    OnAfterUpdate();

    return doUpdate;
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  protected void SetDebugDraw(
    Color debugBoundingBoxColor,
    bool doDrawDebugBoundingBox)
  {
    DebugBoundingBoxColor = debugBoundingBoxColor;
    DoDrawDebugBoundingBox = doDrawDebugBoundingBox;
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  public virtual void DrawGizmos()
  {
  }
}
