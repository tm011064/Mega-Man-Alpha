using UnityEngine;

public class LaserGunAimContainer
{
  private GameObject _laserGunAimGameObject;

  private LineRenderer _laserLineRenderer;

  private SpriteRenderer _laserDotSpriteRenderer;

  private float _currentAimAngle;

  public void Activate()
  {
    if (!_laserGunAimGameObject.activeSelf)
    {
      _laserGunAimGameObject.SetActive(true);
    }
  }

  public void Deactivate()
  {
    if (_laserGunAimGameObject.activeSelf)
    {
      _laserGunAimGameObject.SetActive(false);
    }
  }

  public void UpdateAimAngle(Vector3 aimPosition)
  {
    _laserLineRenderer.SetPosition(0, Vector3.zero);

    _laserLineRenderer.SetPosition(1, aimPosition);

    _laserDotSpriteRenderer.transform.localPosition = aimPosition;
  }

  public void Initialize(Transform laserGunAimGameObjectTransform)
  {
    Logger.Assert(laserGunAimGameObjectTransform != null, "Player controller is expected to have a LaserGunAim child object. If this is no longer needed, remove this line in code.");

    _laserLineRenderer = laserGunAimGameObjectTransform.GetComponent<LineRenderer>();

    _laserDotSpriteRenderer = laserGunAimGameObjectTransform.GetComponentInChildren<SpriteRenderer>();

    _laserGunAimGameObject = laserGunAimGameObjectTransform.gameObject;

    _laserGunAimGameObject.SetActive(false); // we only want to activate this when the player performs the attack.
  }
}