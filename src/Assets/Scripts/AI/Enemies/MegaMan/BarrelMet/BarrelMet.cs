using System.Collections.Generic;
using UnityEngine;

public class BarrelMet : BaseMonoBehaviour, IObjectPoolBehaviour, IPlayerCollidable, ISpawnable
{
  public Vector2 BarrelSpawnLocation;

  public GameObject BarrelMetBarrelPrefab;

  [Tooltip("The number of barrels to create at level load. This value should equal the max expected number of barrels that can be seen on screen at any time")]
  public int BarrelMetBarrelPrefabObjectPoolSize = 12;

  public BallisticTrajectorySettings BarrelThrowTrajectorySettings;

  public int PlayerDamageUnits = 1;

  private BarrelMetBarrel _loadedBarrelMetBarrel;

  private Direction GetFacingDirection()
  {
    return gameObject.transform.localScale.x > 0f ? Direction.Right : Direction.Left;
  }

  public void ShootBarrel()
  {
    if (_loadedBarrelMetBarrel == null)
    {
      return;
    }

    var direction = GetFacingDirection();

    _loadedBarrelMetBarrel.TriggerBarrelThrow(
      BarrelThrowTrajectorySettings,
      direction);

    _loadedBarrelMetBarrel = null;
  }

  public void BarrelLoaded()
  {
    var barrelPrefab = ObjectPoolingManager.Instance.GetObject(
      BarrelMetBarrelPrefab.name,
      gameObject.transform.TransformPoint(BarrelSpawnLocation));

    _loadedBarrelMetBarrel = barrelPrefab.GetComponent<BarrelMetBarrel>();
  }

  protected override void OnDisable()
  {
    if (_loadedBarrelMetBarrel != null)
    {
      ObjectPoolingManager.Instance.Deactivate(_loadedBarrelMetBarrel.gameObject);

      _loadedBarrelMetBarrel = null;
    }

    base.OnDisable();
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    yield return new ObjectPoolRegistrationInfo(BarrelMetBarrelPrefab, BarrelMetBarrelPrefabObjectPoolSize);
  }

  public void OnPlayerCollide(PlayerController playerController)
  {
    playerController.PlayerHealth.ApplyDamage(PlayerDamageUnits);
  }

  public void Reset(Vector3 scale)
  {
    transform.localScale = scale;

    _loadedBarrelMetBarrel = null;
  }
}
