using System.Collections;
using UnityEngine;

public class DeactivateFinishedParticleSystem : MonoBehaviour
{
  private ParticleSystem _particleSystem;

  void OnDisable()
  {
    StopCoroutine(DeactivationTriggerCheck());
  }

  void OnEnable()
  {
    _particleSystem = GetComponent<ParticleSystem>();

    StartCoroutine(DeactivationTriggerCheck());
  }

  IEnumerator DeactivationTriggerCheck()
  {
    var doBreak = false;

    while (!doBreak)
    {
      if (!_particleSystem.isPlaying)
      {
        ObjectPoolingManager.Instance.Deactivate(_particleSystem.gameObject);

        doBreak = true;
      }

      yield return new WaitForSeconds(.2f);
    }
  }
}
