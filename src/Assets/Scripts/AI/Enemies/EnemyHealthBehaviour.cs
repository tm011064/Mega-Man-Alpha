using System.Collections.Generic;
using UnityEngine;

public enum DamageResult
{
  Invincible,

  HealthReduced,

  Destroyed
}

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

    var animator = deathAnimation.GetComponent<Animator>();

    if (animator == null)
    {
      throw new MissingComponentException(
        "Death animation prefab " + DeathAnimationPrefab.name
        + " must contain an Animator component to be able to play the death animation");
    }

    animator.Play(Animator.StringToHash("Enemy Death")); // TODO (Roman): this being hardcoded is flimsy    
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    return new ObjectPoolRegistrationInfo[]
    {
      new ObjectPoolRegistrationInfo(DeathAnimationPrefab, 24) // TODO (Roman): make this a setting value at game manager
    };
  }
}
