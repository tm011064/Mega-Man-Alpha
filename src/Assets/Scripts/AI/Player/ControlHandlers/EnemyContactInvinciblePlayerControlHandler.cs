using UnityEngine;

public class EnemyContactInvinciblePlayerControlHandler : DefaultPlayerControlHandler
{
  private readonly SpriteRendererBlinkTimer _blinkTimer;

  public EnemyContactInvinciblePlayerControlHandler(PlayerController playerController)
    : base(
      playerController,
      duration: playerController.DamageSettings.InvincibleDurationAfterKnockback)
  {
    SetDebugDraw(Color.red, true);

    _blinkTimer = new SpriteRendererBlinkTimer(
      playerController.DamageSettings.SpriteBlinkInterval,
      playerController.SpriteRenderer);
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.Invincible;

    _blinkTimer.Reset();

    return true;
  }

  public override void Dispose()
  {
    _blinkTimer.ShowSprite();

    PlayerController.PlayerState &= ~PlayerState.Invincible;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    _blinkTimer.Update();

    return base.DoUpdate();
  }
}
