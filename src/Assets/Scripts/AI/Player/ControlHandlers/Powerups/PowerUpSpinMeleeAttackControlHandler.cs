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
    PlayerController.SpinMeleeAttackBoxCollider.SetActive(false);
    PlayerController.IsPerformingSpinMeleeAttack = false;

#if !FINAL
    PlayerController.Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
#endif
  }

  private bool CanPerformAttack()
  {
    return !PlayerController.IsAttachedToWall
        && !PlayerController.IsCrouching
        && !PlayerController.IsTakingDamage;
  }

  public override void OnAfterStackPeekUpdate()
  {
    if (PlayerController.IsPerformingSpinMeleeAttack)
    {
      if (!CanPerformAttack())
      {
        PlayerController.IsPerformingSpinMeleeAttack = false;

        PlayerController.SpinMeleeAttackBoxCollider.SetActive(false);
      }
    }
    else
    {
      if (PlayerController.SpinMeleeAttackBoxCollider.activeSelf)
      {
        PlayerController.SpinMeleeAttackBoxCollider.SetActive(false);
      }
    }
  }

  protected override bool DoUpdate()
  {
    if (!PlayerController.IsPerformingSpinMeleeAttack
      && CanPerformAttack()
      && (GameManager.InputStateManager.GetButtonState("Attack").ButtonPressState & ButtonPressState.IsDown) != 0)
    {
      if (!PlayerController.SpinMeleeAttackBoxCollider.activeSelf)
      {
        PlayerController.SpinMeleeAttackBoxCollider.SetActive(true);
      }

      PlayerController.IsPerformingSpinMeleeAttack = true;
    }

    if (!PlayerController.IsPerformingSpinMeleeAttack
      && PlayerController.SpinMeleeAttackBoxCollider.activeSelf)
    {
      PlayerController.SpinMeleeAttackBoxCollider.SetActive(false);
    }

    return base.DoUpdate();
  }
}
