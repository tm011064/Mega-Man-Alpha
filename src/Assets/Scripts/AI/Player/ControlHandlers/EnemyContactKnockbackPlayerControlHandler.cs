using UnityEngine;

public class EnemyContactKnockbackPlayerControlHandler : PlayerControlHandler
{
  private readonly SpriteRendererBlinkTimer _blinkTimer;

  private float _distancePerSecond;

  public EnemyContactKnockbackPlayerControlHandler(PlayerController playerController)
    : base(
      playerController,
      new PlayerStateController[] { new EnemyContactKnockbackPlayerStateController(playerController) },
      playerController.DamageSettings.KnockbackDuration)
  {
    SetDebugDraw(Color.red, true);

    _blinkTimer = new SpriteRendererBlinkTimer(
      playerController.DamageSettings.SpriteBlinkInterval,
      playerController.SpriteRenderer);
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.Invincible;
    PlayerController.PlayerState |= PlayerState.EnemyContactKnockback;

    _blinkTimer.Reset();

    _distancePerSecond = (1f / PlayerController.DamageSettings.KnockbackDuration)
      * PlayerController.DamageSettings.KnockbackDistance;

    if (PlayerController.IsFacingRight())
    {
      _distancePerSecond *= -1f;
    }

    return true;
  }

  public override void Dispose()
  {
    _blinkTimer.ShowSprite();

    PlayerController.PlayerState &= ~PlayerState.EnemyContactKnockback;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    _blinkTimer.Update();

    var deltaMovement = new Vector2(
      Time.deltaTime * _distancePerSecond,
      Mathf.Max(
        GetGravityAdjustedVerticalVelocity(
          PlayerController.CharacterPhysicsManager.Velocity,
          PlayerController.AdjustedGravity,
          true),
        PlayerController.JumpSettings.MaxDownwardSpeed)
      * Time.deltaTime);

    PlayerController.CharacterPhysicsManager.Move(deltaMovement);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
