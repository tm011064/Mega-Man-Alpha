using System;
using System.Collections.Generic;

[Serializable]
public class DamageSettings
{
  public float SpriteBlinkInterval = .1f;

  public float KnockbackDuration = .8f;

  public float KnockbackDistance = 8f;

  public float InvincibleDurationAfterKnockback = 2f;

  public IEnumerable<BaseControlHandler> GetControlHandlers(PlayerController playerController)
  {
    yield return new EnemyContactInvinciblePlayerControlHandler(playerController);
    yield return new EnemyContactKnockbackPlayerControlHandler(playerController);
  }
}