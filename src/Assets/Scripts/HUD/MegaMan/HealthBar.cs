using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour, IObjectPoolBehaviour
{
  private GameObject[] _healthBars = new GameObject[0];

  private int _healthBarHeight;

  void OnEnable()
  {
    InitializeHealthBars();

    GameManager.Instance.Player.PlayerHealth.HealthChanged += OnHealthChanged;
  }

  void OnDisable()
  {
    GameManager.Instance.Player.PlayerHealth.HealthChanged -= OnHealthChanged;
  }

  private void InitializeHealthBars()
  {
    var playerController = GameManager.Instance.Player;

    var objectPoolingManager = ObjectPoolingManager.Instance;

    for (var i = 0; i < _healthBars.Length; i++)
    {
      objectPoolingManager.Deactivate(_healthBars[i]);
    }

    _healthBars = new GameObject[playerController.PlayerHealthSettings.HealthUnits];

    UpdateHealthBars(playerController.PlayerHealthSettings.HealthUnits);
  }

  private void UpdateHealthBars(int totalFullBars)
  {
    var objectPoolingManager = ObjectPoolingManager.Instance;

    for (var i = 0; i < _healthBars.Length; i++)
    {
      var barName = i <= totalFullBars
        ? "Full Bar"
        : "Empty Bar";

      var position = new Vector3(0f, _healthBarHeight * i, transform.position.z);

      position = transform.TransformPoint(position);

      if (_healthBars[i] == null
        || _healthBars[i].activeSelf == false)
      {
        _healthBars[i] = objectPoolingManager.GetObject(barName, position);
      }
      else if (!_healthBars[i].name.StartsWith(barName, StringComparison.OrdinalIgnoreCase))
      {
        objectPoolingManager.Deactivate(_healthBars[i]);

        _healthBars[i] = objectPoolingManager.GetObject(
          barName,
          position);
      }

      _healthBars[i].transform.parent = transform;
    }
  }

  private void OnHealthChanged(int healthUnits)
  {
    UpdateHealthBars(healthUnits);
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    var playerController = GameManager.Instance.Player;

    var fullHealthBar = this.GetChildGameObject("Full Bar");

    _healthBarHeight = Mathf.RoundToInt(fullHealthBar.GetComponent<SpriteRenderer>().sprite.rect.height);

    yield return new ObjectPoolRegistrationInfo(fullHealthBar, playerController.PlayerHealthSettings.HealthUnits);

    fullHealthBar.SetActive(false);

    var emptyHealthBar = this.GetChildGameObject("Empty Bar");

    yield return new ObjectPoolRegistrationInfo(emptyHealthBar, playerController.PlayerHealthSettings.HealthUnits);

    emptyHealthBar.SetActive(false);
  }
}