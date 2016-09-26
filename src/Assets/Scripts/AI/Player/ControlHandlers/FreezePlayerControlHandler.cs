using UnityEngine;

public class FreezePlayerControlHandler : DefaultPlayerControlHandler
{
  private TranslateTransformAction _translateTransformAction;

  private readonly Vector3? _playerTranslationVector;

  private readonly EasingType _playerTranslationEasingType;

  public FreezePlayerControlHandler(
    PlayerController playerController,
    float suspendPhysicsTime,
    int animationShortNameHash,
    Vector3? playerTranslationVector = null,
    EasingType playerTranslationEasingType = EasingType.Linear)
    : base(
      playerController,
      new PlayerStateController[] 
      { 
        new PlaySpecificAnimationPlayerStateController(playerController, animationShortNameHash)
      },
      suspendPhysicsTime)
  {
    SetDebugDraw(Color.red, true);

    _playerTranslationVector = playerTranslationVector;
    _playerTranslationEasingType = playerTranslationEasingType;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.State.Set(PlayerState.Invincible);
    PlayerController.State.Set(PlayerState.Locked);

    if (_playerTranslationVector.HasValue)
    {
      _translateTransformAction = TranslateTransformAction.Start(
        PlayerController.transform.position,
        PlayerController.transform.position + _playerTranslationVector.Value,
        Duration,
        _playerTranslationEasingType,
        GameManager.Instance.Easing);
    }

    ResetOverrideEndTime();

    return true;
  }

  public override void Dispose()
  {
    PlayerController.State.Unset(PlayerState.Invincible);
    PlayerController.State.Unset(PlayerState.Locked);
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (Time.time <= OverrideEndTime)
    {
      if (_translateTransformAction != null)
      {
        PlayerController.transform.position = _translateTransformAction.GetPosition();
      }

      return ControlHandlerAfterUpdateStatus.KeepAlive;
    }

    return base.DoUpdate();
  }
}
