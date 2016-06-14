using UnityEngine;

public class PowerUpSpinMeleeAttackControlHandler : DefaultPlayerControlHandler
{
  public PowerUpSpinMeleeAttackControlHandler(PlayerController playerController)
    : base(playerController)
  {
#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, .75f, 1f);
#endif
  }
  public override void Dispose()
  {
    PlayerController.SpinMeleeSettings.SpinMeleeAttackBoxCollider.SetActive(false);
    PlayerController.PlayerState &= ~PlayerState.PerformingSpinMeleeAttack;

#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
#endif
  }

  private bool CanPerformAttack()
  {
    return (PlayerController.PlayerState & PlayerState.AttachedToWall) == 0
        && (PlayerController.PlayerState & PlayerState.Crouching) == 0
        && (PlayerController.PlayerState & PlayerState.TakingDamage) == 0;
  }

  public override void OnAfterStackPeekUpdate()
  {
    if ((PlayerController.PlayerState & PlayerState.PerformingSpinMeleeAttack) != 0)
    {
      if (!CanPerformAttack())
      {
        PlayerController.PlayerState &= ~PlayerState.PerformingSpinMeleeAttack;

        PlayerController.SpinMeleeSettings.SpinMeleeAttackBoxCollider.SetActive(false);
      }
    }
    else
    {
      if (PlayerController.SpinMeleeSettings.SpinMeleeAttackBoxCollider.activeSelf)
      {
        PlayerController.SpinMeleeSettings.SpinMeleeAttackBoxCollider.SetActive(false);
      }
    }
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if ((PlayerController.PlayerState & PlayerState.PerformingSpinMeleeAttack) == 0
      && CanPerformAttack()
      && (GameManager.InputStateManager.GetButtonState("Attack").ButtonPressState & ButtonPressState.IsDown) != 0)
    {
      if (!PlayerController.SpinMeleeSettings.SpinMeleeAttackBoxCollider.activeSelf)
      {
        PlayerController.SpinMeleeSettings.SpinMeleeAttackBoxCollider.SetActive(true);
      }

      PlayerController.PlayerState |= PlayerState.PerformingSpinMeleeAttack;
    }

    if ((PlayerController.PlayerState & PlayerState.PerformingSpinMeleeAttack) == 0
      && PlayerController.SpinMeleeSettings.SpinMeleeAttackBoxCollider.activeSelf)
    {
      PlayerController.SpinMeleeSettings.SpinMeleeAttackBoxCollider.SetActive(false);
    }

    return base.DoUpdate();
  }
}
