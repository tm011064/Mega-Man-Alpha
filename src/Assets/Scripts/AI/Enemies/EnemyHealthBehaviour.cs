using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBehaviour : MonoBehaviour, IObjectPoolBehaviour
{
  public int HealthUnits = 1;

  public GameObject DeathAnimationPrefab;

  public Vector2 DeathAnimationPrefabOffset = Vector2.zero;

  private bool _isInvincible;

  private int _currentHealthUnits;

  void OnEnable()
  {
    _currentHealthUnits = HealthUnits;
  }

  public void MakeInvincible()
  {
    _isInvincible = true;
  }

  public void MakeVulnerable()
  {
    _isInvincible = false;
  }

  public bool IsInvincible()
  {
    return _isInvincible;
  }

  public DamageResult ApplyDamage(int healthUnitsToDeduct)
  {
    if (_isInvincible)
    {
      return DamageResult.Invincible;
    }

    _currentHealthUnits -= healthUnitsToDeduct;

    if (_currentHealthUnits <= 0)
    {
      ObjectPoolingManager.Instance.Deactivate(gameObject);

      PlayDeathAnimation();

      return DamageResult.Destroyed;
    }

    return DamageResult.HealthReduced;
  }

  private void PlayDeathAnimation()
  {
    var deathAnimation = ObjectPoolingManager.Instance.GetObject(
      DeathAnimationPrefab.name,
      gameObject.transform.position + (Vector3)DeathAnimationPrefabOffset);

    if (deathAnimation == null)
    {
      return;
    }

    var animator = deathAnimation.GetComponent<Animator>();

    Logger.Assert(
      animator != null,
      "Death animation prefab " + DeathAnimationPrefab.name
      + " must contain an Animator component to be able to play the death animation");

    animator.Play(Animator.StringToHash("Enemy Death"));
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(
        DeathAnimationPrefab,
        GameManager.Instance.GameSettings.ObjectPoolSettings.TotalEnemyDeathAnimations);
  }
}
